namespace CollectibleDiecast.Ordering.API.Application.Commands;
using CollectibleDiecast.Ordering.API.Application.Models;

public record CreateOrderDraftCommand(string BuyerId, IEnumerable<BasketItem> Items) : IRequest<OrderDraftDTO>;
