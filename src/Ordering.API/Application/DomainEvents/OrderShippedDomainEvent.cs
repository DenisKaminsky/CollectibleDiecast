using Order = CollectibleDiecast.Ordering.API.Data.Models.Order;

namespace CollectibleDiecast.Ordering.API.Application.DomainEvents;

public class OrderShippedDomainEvent : INotification
{
    public Order Order { get; }

    public OrderShippedDomainEvent(Order order)
    {
        Order = order;
    }
}
