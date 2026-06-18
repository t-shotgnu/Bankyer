using Bankyer.Domain.ValueObjects;
using Bankyer.Shared.Events;

namespace Bankyer.Domain;

public record MoneyDepositedEvent(Guid Id, decimal Amount, Currency Currency) : IDomainEvent;