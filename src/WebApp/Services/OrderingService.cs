namespace CollectibleDiecast.WebApp.Services;

public class OrderingService
{
    private readonly HttpClient _httpClient;

    public OrderingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private readonly string remoteServiceBaseUrl = "/api/Orders/";

    public Task<OrderRecord[]> GetOrders()
    {
        return _httpClient.GetFromJsonAsync<OrderRecord[]>(remoteServiceBaseUrl)!;
    }

    public Task CreateOrder(CreateOrderRequest request, Guid requestId)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, remoteServiceBaseUrl);
        requestMessage.Headers.Add("x-requestid", requestId.ToString());
        requestMessage.Content = JsonContent.Create(request);
        return _httpClient.SendAsync(requestMessage);
    }
}

public record OrderRecord(
    int OrderNumber,
    DateTime Date,
    string Status,
    decimal Total);
