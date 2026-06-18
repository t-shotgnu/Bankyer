using Bankyer.Domain.ValueObjects;

namespace Bankyer.Application.Commands.CreateAccount;

public record CreateAccountCommand(decimal InitialAmount, Currency Currency);

