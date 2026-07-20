using Bankyer.Domain.ValueObjects;
using Bankyer.Shared.Events;

namespace Bankyer.Domain;

[EventTypeName("account.opened")]
public record AccountOpenedEvent(Guid Id, Currency Currency) : IDomainEvent;