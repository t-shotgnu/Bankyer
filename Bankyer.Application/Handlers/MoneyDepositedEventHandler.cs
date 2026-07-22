using Bankyer.Domain;
using Bankyer.Infrastructure.Database;
using Bankyer.Shared.Events;

namespace Bankyer.Application.Handlers;

public class MoneyDepositedEventHandler(AppDbContext dbContext) : IEventHandler<MoneyDepositedEvent>
{
    public async Task HandleAsync(MoneyDepositedEvent @event)
    {
        var account = await dbContext.Accounts.FindAsync(@event.Id);
        if (account is null)
        {
            throw new InvalidOperationException($"Account {@event.Id} projection was not found.");
        }

        account.Balance += @event.Money.Amount;
        await dbContext.SaveChangesAsync();
    }
}
