namespace Bankyer.Shared;

public interface IEventTypeResolver
{
    Type? Resolve(string eventName);
}