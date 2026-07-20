namespace Bankyer.Domain;

public enum WithdrawError
{
    InsufficientFunds,
    InvalidCurrency,
    AmountIsNotPositive,
}