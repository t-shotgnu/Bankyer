using Bankyer.Domain.Aggregates;
using Bankyer.Shared.Events;

namespace Bankyer.Shared;

public class EventTypeResolver : IEventTypeResolver
{
    private readonly Dictionary<string, Type> _typeCache;

    public EventTypeResolver()
    {
        _typeCache = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IDomainEvent).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .ToDictionary(t => t.Name, t => t);
    }

    public Type? Resolve(string eventName)
    {
        return _typeCache.GetValueOrDefault(eventName);
    }
}