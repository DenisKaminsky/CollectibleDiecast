using System.Security.Claims;
using CollectibleDiecast.Basket.API.Repositories;
using CollectibleDiecast.Basket.API.Grpc;
using CollectibleDiecast.Basket.API.Model;
using CollectibleDiecast.Basket.UnitTests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using BasketItem = CollectibleDiecast.Basket.API.Model.BasketItem;

namespace CollectibleDiecast.Basket.UnitTests;

[TestClass]
public class BasketServiceTests
{
    [TestMethod]
    public async Task GetBasketReturnsEmptyForNoUser()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var serverCallContext = TestServerCallContext.Create();
        serverCallContext.SetUserState("__HttpContext", new DefaultHttpContext());

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.AreEqual(response.Items.Count(), 0);
    }

    [TestMethod]
    public async Task GetBasketReturnsItemsForValidUserId()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        List<BasketItem> items = [new BasketItem { Id = "some-id" }];
        mockRepository.GetBasketAsync("1").Returns(Task.FromResult(new CustomerBasket { BuyerId = "1", Items = items }));
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var serverCallContext = TestServerCallContext.Create();
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "1")]));
        serverCallContext.SetUserState("__HttpContext", httpContext);

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.AreEqual(response.Items.Count(), 1);
    }

    [TestMethod]
    public async Task GetBasketReturnsEmptyForInvalidUserId()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        List<BasketItem> items = [new BasketItem { Id = "some-id" }];
        mockRepository.GetBasketAsync("1").Returns(Task.FromResult(new CustomerBasket { BuyerId = "1", Items = items }));
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var serverCallContext = TestServerCallContext.Create();
        var httpContext = new DefaultHttpContext();
        serverCallContext.SetUserState("__HttpContext", httpContext);

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.AreEqual(response.Items.Count(), 0);
    }
}
