using CollectibleDiecast.Basket.API.Grpc;
using GrpcBasketItem = CollectibleDiecast.Basket.API.Grpc.BasketItem;
using GrpcBasketClient = CollectibleDiecast.Basket.API.Grpc.Basket.BasketClient;

namespace CollectibleDiecast.WebApp.Services;

public class BasketService
{
    private readonly GrpcBasketClient _basketClient;

    public BasketService(GrpcBasketClient basketClient)
    {
        _basketClient = basketClient;
    }

    public async Task<IReadOnlyCollection<BasketItemWithQuantity>> GetBasketAsync()
    {
        var result = await _basketClient.GetBasketAsync(new GetBasketRequest());
        return MapToBasket(result);
    }

    public async Task DeleteBasketAsync()
    {
        await _basketClient.DeleteBasketAsync(new DeleteBasketRequest());
    }

    public async Task UpdateBasketAsync(IReadOnlyCollection<BasketItemWithQuantity> basket)
    {
        var updatePayload = new UpdateBasketRequest();

        foreach (var item in basket)
        {
            var updateItem = new GrpcBasketItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            };
            updatePayload.Items.Add(updateItem);
        }

        await _basketClient.UpdateBasketAsync(updatePayload);
    }

    private static List<BasketItemWithQuantity> MapToBasket(CustomerBasketResponse response)
    {
        var result = new List<BasketItemWithQuantity>();
        foreach (var item in response.Items)
        {
            result.Add(new BasketItemWithQuantity(item.ProductId, item.Quantity));
        }

        return result;
    }
}

public record BasketItemWithQuantity(int ProductId, int Quantity);
