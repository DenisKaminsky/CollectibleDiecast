namespace CollectibleDiecast.Catalog.API.IntegrationEvents;

public sealed class CatalogIntegrationEventService : ICatalogIntegrationEventService, IDisposable
{
    private readonly ILogger<CatalogIntegrationEventService> _logger;
    private readonly IEventBus _eventBus;
    private readonly CatalogContext _catalogContext;
    private readonly IIntegrationEventLogService _integrationEventLogService;

    public CatalogIntegrationEventService(ILogger<CatalogIntegrationEventService> logger,
        IEventBus eventBus,
        CatalogContext catalogContext,
        IIntegrationEventLogService integrationEventLogService)
    {
        _logger = logger;
        _eventBus = eventBus;
        _catalogContext = catalogContext;
        _integrationEventLogService = integrationEventLogService;
    }

    public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
    {
        try
        {
            _logger.LogInformation("Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})", evt.Id, evt);

            await _integrationEventLogService.MarkEventAsInProgressAsync(evt.Id);
            await _eventBus.PublishAsync(evt);
            await _integrationEventLogService.MarkEventAsPublishedAsync(evt.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", evt.Id, evt);
            await _integrationEventLogService.MarkEventAsFailedAsync(evt.Id);
        }
    }

    public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
    {
        _logger.LogInformation("CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", evt.Id);

        var strategy = _catalogContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _catalogContext.Database.BeginTransactionAsync();

            await _catalogContext.SaveChangesAsync();
            await _integrationEventLogService.SaveEventAsync(evt, _catalogContext.Database.CurrentTransaction);

            await transaction.CommitAsync();
        });
    }


    #region dispose
    private volatile bool disposedValue;

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                (_integrationEventLogService as IDisposable)?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
