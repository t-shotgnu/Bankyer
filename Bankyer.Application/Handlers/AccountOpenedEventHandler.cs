using Bankyer.Domain;
using Bankyer.Domain.ValueObjects;
using Bankyer.Infrastructure.Database;
using Bankyer.Infrastructure.Database.Entities;
using Bankyer.Shared.Events;

namespace Bankyer.Application.Handlers;

public class AccountOpenedEventHandler(AppDbContext dbContext) : IEventHandler<AccountOpenedEvent>
{
    public async Task HandleAsync(AccountOpenedEvent @event)
    {
        var account = await dbContext.Accounts.FindAsync(@event.Id);
        if (account is not null)
        {
            return;
        }

        dbContext.Accounts.Add(new AccountEntity
        {
            Id = @event.Id,
            Status = AccountStatus.Active,
            Balance = 0m,
        });

        await dbContext.SaveChangesAsync();
    }
}
