using Bankyer.Domain.ValueObjects;

namespace Bankyer.Application.Commands.Withdraw;

public record WithdrawCommand(Guid Id, decimal Amount, Currency Currency);

