using CollectibleDiecast.EventBus.Abstractions;

namespace CollectibleDiecast.WebApp.Services.OrderStatus.IntegrationEvents;

public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    private readonly OrderStatusNotificationService _orderStatusNotificationService;
    private readonly ILogger<OrderStatusChangedToPaidIntegrationEventHandler> _logger;

    public OrderStatusChangedToPaidIntegrationEventHandler(
        OrderStatusNotificationService orderStatusNotificationService,
        ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
    {
        _orderStatusNotificationService = orderStatusNotificationService;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
        await _orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
    }
}
