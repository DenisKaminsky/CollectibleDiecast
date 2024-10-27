using Order = CollectibleDiecast.Ordering.API.Data.Models.Order;

namespace CollectibleDiecast.Ordering.API.Application.DomainEvents;

public class OrderCancelledDomainEvent : INotification
{
    public Order Order { get; }

    public OrderCancelledDomainEvent(Order order)
    {
        Order = order;
    }
}

