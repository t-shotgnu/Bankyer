namespace Bankyer.Shared.Events;

public interface IEventBus
{
    Task PublishAsync(IDomainEvent @event);
}