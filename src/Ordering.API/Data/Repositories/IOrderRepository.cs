using Order = CollectibleDiecast.Ordering.API.Data.Models.Order;

namespace CollectibleDiecast.Ordering.API.Data.Repositories;

public interface IOrderRepository : IRepository
{
    Order Add(Order order);

    void Update(Order order);

    Task<Order> GetAsync(int orderId);
}
