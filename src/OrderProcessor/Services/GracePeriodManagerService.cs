using CollectibleDiecast.EventBus.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;
using CollectibleDiecast.OrderProcessor.Events;

namespace CollectibleDiecast.OrderProcessor.Services
{
    public class GracePeriodManagerService : BackgroundService
    {
        private readonly BackgroundTaskOptions _options;
        private readonly IEventBus _eventBus;
        private readonly ILogger<GracePeriodManagerService> _logger;
        private readonly NpgsqlDataSource _dataSource;

        public GracePeriodManagerService(
            IOptions<BackgroundTaskOptions> options,
            IEventBus eventBus,
            ILogger<GracePeriodManagerService> logger,
            NpgsqlDataSource dataSource)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _eventBus = eventBus;
            _logger = logger;
            _dataSource = dataSource;
        }
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delayTime = TimeSpan.FromSeconds(_options.CheckUpdateTime);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("GracePeriodManagerService is starting.");
                stoppingToken.Register(() => _logger.LogDebug("GracePeriodManagerService background task is stopping."));
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("GracePeriodManagerService background task is doing background work.");
                }

                await CheckConfirmedGracePeriodOrders();

                await Task.Delay(delayTime, stoppingToken);
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("GracePeriodManagerService background task is stopping.");
            }
        }

        private async Task CheckConfirmedGracePeriodOrders()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Checking confirmed grace period orders");
            }

            var orderIds = await GetConfirmedGracePeriodOrders();

            foreach (var orderId in orderIds)
            {
                var confirmGracePeriodEvent = new GracePeriodConfirmedIntegrationEvent(orderId);

                _logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", confirmGracePeriodEvent.Id, confirmGracePeriodEvent);

                await _eventBus.PublishAsync(confirmGracePeriodEvent);
            }
        }

        private async ValueTask<List<int>> GetConfirmedGracePeriodOrders()
        {
            try
            {
                using var conn = _dataSource.CreateConnection();
                using var command = conn.CreateCommand();
                command.CommandText = """
                    SELECT "Id"
                    FROM ordering.orders
                    WHERE CURRENT_TIMESTAMP - "OrderDate" >= @GracePeriodTime AND "OrderStatus" = 'Submitted'
                    """;
                command.Parameters.AddWithValue("GracePeriodTime", TimeSpan.FromMinutes(_options.GracePeriodTime));

                List<int> ids = new List<int>();

                await conn.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ids.Add(reader.GetInt32(0));
                }

                return ids;
            }
            catch (NpgsqlException exception)
            {
                _logger.LogError(exception, "Fatal error establishing database connection");
            }

            return new List<int>();
        }
    }
}
