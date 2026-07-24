namespace Bankyer.Application.Commands.Deposit;

public record DepositCommand(Guid Id, decimal Amount, string Currency);
