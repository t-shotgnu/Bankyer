using Bankyer.Domain.ValueObjects;
using Bankyer.Shared.Events;

namespace Bankyer.Domain;

public record AccountOpenedEvent(Guid Id, Currency Currency) : IDomainEvent;