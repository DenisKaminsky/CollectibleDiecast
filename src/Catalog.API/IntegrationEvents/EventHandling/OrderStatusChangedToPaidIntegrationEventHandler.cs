﻿namespace CollectibleDiecast.Catalog.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    private readonly CatalogContext _catalogContext;
    private readonly ILogger<OrderStatusChangedToPaidIntegrationEventHandler> _logger;

    public OrderStatusChangedToPaidIntegrationEventHandler(CatalogContext catalogContext,
        ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
    {
        _catalogContext = catalogContext;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        //we're not blocking stock/inventory
        foreach (var orderStockItem in @event.OrderStockItems)
        {
            var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);

            catalogItem.RemoveStock(orderStockItem.Units);
        }

        await _catalogContext.SaveChangesAsync();
    }
}
