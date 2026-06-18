using Bankyer.Domain.ValueObjects;

namespace Bankyer.Application.Commands.Deposit;

public record DepositCommand(Guid Id, decimal Amount, Currency Currency);

