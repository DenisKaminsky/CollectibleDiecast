using CollectibleDiecast.Basket.API.Repositories;
using CollectibleDiecast.Basket.API.IntegrationEvents.EventHandling.Events;

namespace CollectibleDiecast.Basket.API.IntegrationEvents.EventHandling;

public class OrderStartedIntegrationEventHandler: IIntegrationEventHandler<OrderStartedIntegrationEvent>
{
    private readonly IBasketRepository _repository;
    private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

    public OrderStartedIntegrationEventHandler(IBasketRepository repository, ILogger<OrderStartedIntegrationEventHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(OrderStartedIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        await _repository.DeleteBasketAsync(@event.UserId);
    }
}
