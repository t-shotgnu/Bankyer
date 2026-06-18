using Bankyer.Application.Commands.CreateAccount;
using Bankyer.Domain.Aggregates;
using Bankyer.Domain.ValueObjects;
using Bankyer.Shared;

namespace Bankyer.Application.Commands.Deposit;

public class DepositCommandHandler(IEventStore eventStore)
{
    public async Task<Guid> Handle(DepositCommand command, CancellationToken cancellationToken = default)
    {
        var history = await eventStore.GetEventsAsync(command.Id);
        if (!history.Any())
        {
            throw new InvalidOperationException("Account not found.");
        }

        var account = new Account();
        account.LoadFromHistory(history);

        var amountToDeposit = new Money(command.Amount, command.Currency);
        account.Deposit(amountToDeposit);

        await eventStore.SaveEventsAsync(
            account.Id,
            "Deposit",
            account.DomainEvents,
            account.Version
        );

        account.ClearChanges();

        return account.Id;
    }
}