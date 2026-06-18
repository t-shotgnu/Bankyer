namespace Bankyer.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
}