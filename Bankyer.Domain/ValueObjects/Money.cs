namespace Bankyer.Domain.ValueObjects;

public record Money
{
    public Money(decimal amount, Currency currency)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

}