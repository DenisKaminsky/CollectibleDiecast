namespace CollectibleDiecast.PaymentProcessor.IntegrationEvents.Events;

public record OrderPaymentSucceededIntegrationEvent(int OrderId) : IntegrationEvent;
