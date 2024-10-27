using CollectibleDiecast.WebApp.Components.Catalog;

namespace CollectibleDiecast.WebApp.Helpers;

public static class ItemHelper
{
    public static string Url(CatalogItem item)
        => $"item/{item.Id}";
}
