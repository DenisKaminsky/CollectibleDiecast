using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CollectibleDiecast.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CollectibleDiecast.EventBus.Extensions;

public static class EventBusBuilderExtensions
{
    public static IEventBusBuilder ConfigureJsonOptions(this IEventBusBuilder eventBusBuilder, Action<JsonSerializerOptions> configure)
    {
        eventBusBuilder.Services.Configure<EventBusSubscriptionInfo>(o =>
        {
            configure(o.JsonSerializerOptions);
        });

        return eventBusBuilder;
    }

    public static IEventBusBuilder AddSubscription<TIntegrationEvent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(this IEventBusBuilder eventBusBuilder)
        where TIntegrationEvent : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<TIntegrationEvent>
    {
        // Use keyed services to register multiple handlers for the same event type
        // the consumer can use IKeyedServiceProvider.GetKeyedService<IIntegrationEventHandler>(typeof(T)) to get all
        // handlers for the event type.
        eventBusBuilder.Services.AddKeyedTransient<IIntegrationEventHandler, THandler>(typeof(TIntegrationEvent));

        eventBusBuilder.Services.Configure<EventBusSubscriptionInfo>(o =>
        {
            // This list is used to subscribe to events from the underlying message broker implementation.
            o.EventTypes[typeof(TIntegrationEvent).Name] = typeof(TIntegrationEvent);
        });

        return eventBusBuilder;
    }
}
