using Bankyer.Domain.Aggregates;
using Bankyer.Domain.ValueObjects;
using Bankyer.Shared;

namespace Bankyer.Application.Commands.Withdraw;

public class WithdrawCommandHandler(IEventStore eventStore)
{
    public async Task<Guid?> Handle(WithdrawCommand request, CancellationToken cancellationToken = default)
    {
        var history = await eventStore.GetEventsAsync(request.Id);
        if (!history.Any())
        {
            return null;
        }

        var account = new Account();
        account.LoadFromHistory(history);

        var amountToWithdraw = new Money(request.Amount, request.Currency);
        account.Withdraw(amountToWithdraw);

        await eventStore.SaveEventsAsync(
            account.Id,
            "Account",
            account.DomainEvents,
            account.Version
        );

        account.ClearChanges();

        return account.Id;
    }
}