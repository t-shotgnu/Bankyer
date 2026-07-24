namespace Bankyer.Application.Commands.CreateAccount;

public record CreateAccountCommand(decimal InitialAmount, string Currency, string UserId);
