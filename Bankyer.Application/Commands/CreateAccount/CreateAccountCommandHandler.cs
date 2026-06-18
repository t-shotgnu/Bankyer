using Bankyer.Domain.Aggregates;
using Bankyer.Domain.ValueObjects;
using Bankyer.Infrastructure.Database;
using Bankyer.Infrastructure.Database.Entities;
using Bankyer.Shared;

namespace Bankyer.Application.Commands.CreateAccount;

public class CreateAccountCommandHandler(AppDbContext dbContext, IEventStore eventStore)
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken = default)
    {
        var account = new Account(Guid.NewGuid(), request.Currency);
        
        dbContext.Accounts.Add(new AccountEntity
        {
            Id = account.Id,
            Status = AccountStatus.Active,
        });
        
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