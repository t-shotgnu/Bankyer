using Bankyer.Domain.ValueObjects;
using Bankyer.Shared.Events;

namespace Bankyer.Domain;

[EventTypeName("account.money.deposited")]
public record MoneyDepositedEvent(Guid Id, Money Money) : IDomainEvent;