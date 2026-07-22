using Bankyer.Domain.Aggregates;
using Bankyer.Domain.ValueObjects;
using Bankyer.Shared;

namespace Bankyer.Application.Commands.CreateAccount;

public class CreateAccountCommandHandler(IEventStore eventStore)
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken = default)
    {
        var account = Account.Create(Guid.NewGuid(), request.Currency);
        
        if (request.InitialAmount > 0)
        {
            var initialMoney = new Money(request.InitialAmount, request.Currency);
            account.Deposit(initialMoney);
        }

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
