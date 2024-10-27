using System.Text.Json.Serialization;
using CollectibleDiecast.Basket.API.Repositories;
using CollectibleDiecast.Basket.API.IntegrationEvents.EventHandling;
using CollectibleDiecast.Basket.API.IntegrationEvents.EventHandling.Events;
using CollectibleDiecast.EventBus.Extensions;
using CollectibleDiecast.EventBusRabbitMQ;

namespace CollectibleDiecast.Basket.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddDefaultAuthentication();

        builder.AddRedisClient("redis");

        builder.Services.AddSingleton<IBasketRepository, RedisBasketRepository>();

        builder.AddRabbitMqEventBus("eventbus")
               .AddSubscription<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>()
               .ConfigureJsonOptions(options => options.TypeInfoResolverChain.Add(IntegrationEventContext.Default));
    }
}

[JsonSerializable(typeof(OrderStartedIntegrationEvent))]
partial class IntegrationEventContext : JsonSerializerContext
{

}
