using Bankyer.Shared;
using Bankyer.Shared.Events;
using System.Reflection;

namespace Bankyer.Infrastructure;

public class EventTypeResolver : IEventTypeResolver
{
    private readonly Dictionary<string, Type> _typeCache;

    public EventTypeResolver()
    {
        _typeCache = new Dictionary<string, Type>(StringComparer.Ordinal);

        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(GetLoadableTypes)
                     .Where(t => typeof(IDomainEvent).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false }))
        {
            _typeCache.TryAdd(GetEventName(type), type);
            _typeCache.TryAdd(type.Name, type);
        }
    }

    public Type? Resolve(string eventName)
    {
        return _typeCache.GetValueOrDefault(eventName);
    }

    public string GetName(Type eventType)
    {
        return GetEventName(eventType);
    }

    private static string GetEventName(Type eventType)
    {
        var attribute = eventType.GetCustomAttribute<EventTypeNameAttribute>();
        return attribute?.NameParts is { Length: > 0 } nameParts
            ? string.Join(".", nameParts)
            : eventType.Name;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.OfType<Type>();
        }
    }
}