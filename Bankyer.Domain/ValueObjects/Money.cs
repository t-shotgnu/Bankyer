using Bankyer.Shared;
using Bankyer.Shared.Exceptions;

namespace Bankyer.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (amount <= 0)
        {
            throw new DomainViolationException([new DomainRuleViolation("money.amount.invalid", "Amount must be a positive value")]);
        }
        Amount = amount;
        Currency = currency;
    }
}