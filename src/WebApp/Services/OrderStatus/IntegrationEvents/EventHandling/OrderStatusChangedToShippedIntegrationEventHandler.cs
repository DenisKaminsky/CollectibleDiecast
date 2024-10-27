using CollectibleDiecast.EventBus.Abstractions;

namespace CollectibleDiecast.WebApp.Services.OrderStatus.IntegrationEvents;

public class OrderStatusChangedToShippedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToShippedIntegrationEvent>
{
    private readonly OrderStatusNotificationService _orderStatusNotificationService;
    private readonly ILogger<OrderStatusChangedToShippedIntegrationEventHandler> _logger;

    public OrderStatusChangedToShippedIntegrationEventHandler(
        OrderStatusNotificationService orderStatusNotificationService,
        ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger)
    {
        _orderStatusNotificationService = orderStatusNotificationService;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToShippedIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
        await _orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
    }
}
