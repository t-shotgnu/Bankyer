using Bankyer.Shared.Events;

namespace Bankyer.Shared;

public record EventRecord(IDomainEvent Event, DateTime CreatedAt);

