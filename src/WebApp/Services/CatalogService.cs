using System.Web;
using CollectibleDiecast.WebApp.Components.Catalog;

namespace CollectibleDiecast.WebApp.Services;

public class CatalogService: ICatalogService
{
    private readonly HttpClient _httpClient;
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public CatalogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/{id}";
        return _httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }

    public async Task<CatalogResult> GetCatalogItems(int pageIndex, int pageSize, int? brand, int? type)
    {
        var uri = GetAllCatalogItemsUri(remoteServiceBaseUrl, pageIndex, pageSize, brand, type);
        var result = await _httpClient.GetFromJsonAsync<CatalogResult>(uri);
        return result!;
    }

    public async Task<List<CatalogItem>> GetCatalogItems(IEnumerable<int> ids)
    {
        var uri = $"{remoteServiceBaseUrl}items/by?ids={string.Join("&ids=", ids)}";
        var result = await _httpClient.GetFromJsonAsync<List<CatalogItem>>(uri);
        return result!;
    }

    public Task<CatalogResult> GetCatalogItemsWithSemanticRelevance(int page, int take, string text)
    {
        var url = $"{remoteServiceBaseUrl}items/withsemanticrelevance/{HttpUtility.UrlEncode(text)}?pageIndex={page}&pageSize={take}";
        var result = _httpClient.GetFromJsonAsync<CatalogResult>(url);
        return result!;
    }

    public async Task<IEnumerable<CatalogBrand>> GetBrands()
    {
        var uri = $"{remoteServiceBaseUrl}catalogBrands";
        var result = await _httpClient.GetFromJsonAsync<CatalogBrand[]>(uri);
        return result!;
    }

    public async Task<IEnumerable<CatalogItemType>> GetTypes()
    {
        var uri = $"{remoteServiceBaseUrl}catalogTypes";
        var result = await _httpClient.GetFromJsonAsync<CatalogItemType[]>(uri);
        return result!;
    }

    private static string GetAllCatalogItemsUri(string baseUri, int pageIndex, int pageSize, int? brand, int? type)
    {
        string filterQs;

        if (type.HasValue)
        {
            var brandQs = brand.HasValue ? brand.Value.ToString() : string.Empty;
            filterQs = $"/type/{type.Value}/brand/{brandQs}";

        }
        else if (brand.HasValue)
        {
            var brandQs = brand.HasValue ? brand.Value.ToString() : string.Empty;
            filterQs = $"/type/all/brand/{brandQs}";
        }
        else
        {
            filterQs = string.Empty;
        }

        return $"{baseUri}items{filterQs}?pageIndex={pageIndex}&pageSize={pageSize}";
    }
}
