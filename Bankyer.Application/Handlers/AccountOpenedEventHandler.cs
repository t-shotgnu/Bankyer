using Bankyer.Domain;
using Bankyer.Domain.ValueObjects;
using Bankyer.Infrastructure.Database;
using Bankyer.Infrastructure.Database.Entities;
using Bankyer.Shared.Events;
using Microsoft.EntityFrameworkCore;

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
        
        var currencyEntity = await dbContext.Currencies
            .SingleOrDefaultAsync(currency => currency.Code == @event.Currency);
        if (currencyEntity is null)
        {
            throw new InvalidOperationException($"Currency '{@event.Currency}' does not exist.");
        }

        var user = await dbContext.Users.FindAsync(@event.UserId);
        if (user is null)
        {
            throw new InvalidOperationException($"User '{@event.UserId}' does not exist.");
        }

        dbContext.Accounts.Add(new AccountEntity
        {
            Id = @event.Id,
            Status = nameof(AccountStatus.Active),
            Balance = 0m,
            CurrencyCode = currencyEntity.Code,
            Currency = currencyEntity,
            UserId = user.Id,
            User = user,
        });

        await dbContext.SaveChangesAsync();
    }
}
