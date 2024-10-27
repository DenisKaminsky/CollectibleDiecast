using CollectibleDiecast.WebApp.Components.Catalog;

namespace CollectibleDiecast.WebApp.Services;

public interface IProductImageUrlProvider
{
    string GetProductImageUrl(CatalogItem item)
        => GetProductImageUrl(item.Id);

    string GetProductImageUrl(int productId);
}
