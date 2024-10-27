using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CollectibleDiecast.EventBusRabbitMQ;

public static class RabbitMqDependencyInjectionExtensions
{
    // {
    //   "EventBus": {
    //     "SubscriptionClientName": "...",
    //     "RetryCount": 10
    //   }
    // }

    private const string SectionName = "EventBus";

    public static IEventBusBuilder AddRabbitMqEventBus(this IHostApplicationBuilder builder, string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddRabbitMQClient(connectionName, configureConnectionFactory: factory =>
        {
            ((ConnectionFactory)factory).DispatchConsumersAsync = true;
        });
        
        // Options support
        builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection(SectionName));
        
        builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

        // Start consuming messages as soon as the application starts
        builder.Services.AddSingleton<IHostedService>(sp => (RabbitMQEventBus)sp.GetRequiredService<IEventBus>());

        return new EventBusBuilder(builder.Services);
    }

    private class EventBusBuilder : IEventBusBuilder
    {
        private readonly IServiceCollection _services;

        public IServiceCollection Services => _services;

        public EventBusBuilder(IServiceCollection services)
        {
            _services = services;
        }
        
    }
}
