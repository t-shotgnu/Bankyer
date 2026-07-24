using Bankyer.Shared.Events;

namespace Bankyer.Domain;

[EventTypeName("account.opened")]
public record AccountOpenedEvent(Guid Id, string Currency, string UserId) : IDomainEvent;
