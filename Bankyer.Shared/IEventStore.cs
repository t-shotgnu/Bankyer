using Bankyer.Shared.Events;

namespace Bankyer.Shared;

public interface IEventStore
{
    Task<IReadOnlyList<IDomainEvent>> GetEventsAsync(Guid aggregateId);

    Task<IReadOnlyList<EventRecord>> GetEventRecordsAsync(Guid aggregateId);

    Task SaveEventsAsync(Guid aggregateId, string aggregateType, IReadOnlyCollection<IDomainEvent> events, long expectedVersion);
}