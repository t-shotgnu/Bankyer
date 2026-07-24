namespace Bankyer.Application.Commands.Withdraw;

public record WithdrawCommand(Guid Id, decimal Amount, string Currency);
