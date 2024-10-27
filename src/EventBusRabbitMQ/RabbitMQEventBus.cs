using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly.Retry;

namespace CollectibleDiecast.EventBusRabbitMQ;

public sealed class RabbitMQEventBus: IEventBus, IDisposable, IHostedService
{
    private const string ExchangeName = "collectible_diecast_event_bus";

    private readonly ILogger<RabbitMQEventBus> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly ResiliencePipeline _pipeline;
    private readonly EventBusSubscriptionInfo _subscriptionInfo;

    private IConnection _rabbitMQConnection;
    private IModel _consumerChannel;
    private readonly string _queueName;

    public RabbitMQEventBus(
        ILogger<RabbitMQEventBus> logger,
        IServiceProvider serviceProvider,
        IOptions<EventBusOptions> options,
        IOptions<EventBusSubscriptionInfo> subscriptionOptions)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        _pipeline = CreateResiliencePipeline(options.Value.RetryCount);
        _subscriptionInfo = subscriptionOptions.Value;
        _queueName = options.Value.SubscriptionClientName;
    }

    public Task PublishAsync(IntegrationEvent @event)
    {
        var routingKey = @event.GetType().Name;

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, routingKey);
        }

        using var channel = _rabbitMQConnection?.CreateModel() ?? throw new InvalidOperationException("RabbitMQ connection is not open");

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
        }

        channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");

        var body = SerializeMessage(@event);

        return _pipeline.Execute(() =>
        {
            var properties = channel.CreateBasicProperties();
            // messages are persistent and will be saved to disk, which will allow to recover the message in case of crashing
            properties.DeliveryMode = 2;
            
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);
            }

            channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                //If no queue is bound to the routing key, RabbitMQ returns the message to the publisher
                mandatory: true,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        });
    }
    
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        //Create long running background task to process messages async.
        _ = Task.Factory.StartNew(() =>
        {
            try
            {
                _logger.LogInformation("Starting RabbitMQ connection on a background thread");

                _rabbitMQConnection = _serviceProvider.GetRequiredService<IConnection>();
                if (!_rabbitMQConnection.IsOpen)
                {
                    return;
                }

                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace("Creating RabbitMQ consumer channel");
                }

                _consumerChannel = _rabbitMQConnection.CreateModel();

                _consumerChannel.CallbackException += (sender, ea) =>
                {
                    _logger.LogWarning(ea.Exception, "Error with RabbitMQ consumer channel");
                };

                //create direct exchange
                _consumerChannel.ExchangeDeclare(exchange: ExchangeName, type: "direct");

                //create one queue
                _consumerChannel.QueueDeclare(
                    queue: _queueName,
                    //queue will survive a RabbitMQ server restart.
                    durable: true,
                    //queue is accessible by multiple connections and persists beyond this listener
                    exclusive: false,
                    //RabbitMQ will not delete the queue automatically when there are no consumers.
                    autoDelete: false,
                    arguments: null
                );

                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace("Starting RabbitMQ basic consume");
                }

                //create async consumer
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.Received += OnMessageReceived;

                //save consumer for queue
                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);

                foreach (var (eventName, _) in _subscriptionInfo.EventTypes)
                {
                    _consumerChannel.QueueBind(
                        queue: _queueName,
                        exchange: ExchangeName,
                        routingKey: eventName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting RabbitMQ connection");
            }
        },
        TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            if (message.Contains("throw-fake-exception", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            await ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error Processing message \"{Message}\"", message);
        }

        // Even on exception we take the message off the queue. (Probably should be handled with  Dead Letter Exchange (DLX))
        _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);
        }

        await using var scope = _serviceProvider.CreateAsyncScope();

        if (!_subscriptionInfo.EventTypes.TryGetValue(eventName, out var eventType))
        {
            _logger.LogWarning("Unable to resolve event type for event name {EventName}", eventName);
            return;
        }

        // Deserialize the event
        var integrationEvent = DeserializeMessage(message, eventType);

        // Get all the handlers using the event type as the key
        foreach (var handler in scope.ServiceProvider.GetKeyedServices<IIntegrationEventHandler>(eventType))
        {
            await handler.Handle(integrationEvent);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "The 'JsonSerializer.IsReflectionEnabledByDefault' feature switch, which is set to false by default for trimmed .NET apps, ensures the JsonSerializer doesn't use Reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "See above.")]
    private IntegrationEvent DeserializeMessage(string message, Type eventType)
    {
        return JsonSerializer.Deserialize(message, eventType, _subscriptionInfo.JsonSerializerOptions) as IntegrationEvent;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "The 'JsonSerializer.IsReflectionEnabledByDefault' feature switch, which is set to false by default for trimmed .NET apps, ensures the JsonSerializer doesn't use Reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "See above.")]
    private byte[] SerializeMessage(IntegrationEvent @event)
    {
        return JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), _subscriptionInfo.JsonSerializerOptions);
    }

    private static ResiliencePipeline CreateResiliencePipeline(int retryCount)
    {
        var retryOptions = new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<BrokerUnreachableException>().Handle<SocketException>(),
            MaxRetryAttempts = retryCount,
            DelayGenerator = (context) => ValueTask.FromResult(GenerateDelay(context.AttemptNumber))
        };

        return new ResiliencePipelineBuilder()
            .AddRetry(retryOptions)
            .Build();

        static TimeSpan? GenerateDelay(int attempt)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, attempt));
        }
    }

    public void Dispose()
    {
        _consumerChannel?.Dispose();
    }
}
