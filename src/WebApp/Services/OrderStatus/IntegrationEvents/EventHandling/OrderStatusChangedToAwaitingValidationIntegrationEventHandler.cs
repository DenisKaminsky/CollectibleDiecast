using CollectibleDiecast.EventBus.Abstractions;

namespace CollectibleDiecast.WebApp.Services.OrderStatus.IntegrationEvents;

public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
{
    private readonly OrderStatusNotificationService _orderStatusNotificationService;
    private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> _logger;

    public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(
        OrderStatusNotificationService orderStatusNotificationService,
        ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger)
    {
        _orderStatusNotificationService = orderStatusNotificationService;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
        await _orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
    }
}
