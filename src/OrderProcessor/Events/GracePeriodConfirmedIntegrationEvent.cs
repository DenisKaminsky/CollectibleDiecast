namespace CollectibleDiecast.OrderProcessor.Events
{
    using CollectibleDiecast.EventBus.Events;

    public record GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(int orderId) =>
            OrderId = orderId;
    }
}
