using CollectibleDiecast.EventBus.Abstractions;

namespace CollectibleDiecast.WebApp.Services.OrderStatus.IntegrationEvents;

public class OrderStatusChangedToSubmittedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>
{
    private readonly OrderStatusNotificationService _orderStatusNotificationService;
    private readonly ILogger<OrderStatusChangedToSubmittedIntegrationEventHandler> _logger;

    public OrderStatusChangedToSubmittedIntegrationEventHandler(
        OrderStatusNotificationService orderStatusNotificationService,
        ILogger<OrderStatusChangedToSubmittedIntegrationEventHandler> logger)
    {
        _orderStatusNotificationService = orderStatusNotificationService;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToSubmittedIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
        await _orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
    }
}
