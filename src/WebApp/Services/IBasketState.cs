using CollectibleDiecast.WebAppComponents.Catalog;

namespace CollectibleDiecast.WebApp.Services
{
    public interface IBasketState
    {
        public Task<IReadOnlyCollection<BasketItem>> GetBasketItemsAsync();

        public Task AddAsync(CatalogItem item);
    }
}
