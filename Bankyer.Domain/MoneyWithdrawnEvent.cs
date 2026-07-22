using Bankyer.Domain.ValueObjects;
using Bankyer.Shared.Events;

namespace Bankyer.Domain;

[EventTypeName("account.money.withdrawn")]
public record MoneyWithdrawnEvent(Guid Id, Money Money) : IDomainEvent;