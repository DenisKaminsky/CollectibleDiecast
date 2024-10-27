using Microsoft.Extensions.DependencyInjection;

namespace CollectibleDiecast.EventBus.Abstractions;

public interface IEventBusBuilder
{
    public IServiceCollection Services { get; }
}
