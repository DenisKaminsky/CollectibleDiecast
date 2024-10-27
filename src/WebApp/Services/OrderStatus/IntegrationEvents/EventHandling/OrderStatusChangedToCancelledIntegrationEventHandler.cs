using CollectibleDiecast.EventBus.Abstractions;

namespace CollectibleDiecast.WebApp.Services.OrderStatus.IntegrationEvents;

public class OrderStatusChangedToCancelledIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>
{
    private readonly OrderStatusNotificationService _orderStatusNotificationService;
    private readonly ILogger<OrderStatusChangedToCancelledIntegrationEventHandler> _logger;

    public OrderStatusChangedToCancelledIntegrationEventHandler(
        OrderStatusNotificationService orderStatusNotificationService,
        ILogger<OrderStatusChangedToCancelledIntegrationEventHandler> logger)
    {
        _orderStatusNotificationService = orderStatusNotificationService;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToCancelledIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
        await _orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
    }
}
