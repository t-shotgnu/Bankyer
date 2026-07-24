using Bankyer.Shared;
using Bankyer.Shared.Exceptions;

namespace Bankyer.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new DomainViolationException([new DomainRuleViolation("money.amount.invalid", "Amount must be a non-negative value")]);
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainViolationException([new DomainRuleViolation("money.currency.invalid", "Currency is required")]);
        }

        Amount = amount;
        Currency = currency;
    }
}
