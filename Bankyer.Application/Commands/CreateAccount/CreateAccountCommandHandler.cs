using Bankyer.Domain.Aggregates;
using Bankyer.Domain.ValueObjects;
using Bankyer.Infrastructure.Database;
using Bankyer.Shared;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Application.Commands.CreateAccount;

public class CreateAccountCommandHandler(AppDbContext dbContext, IEventStore eventStore)
{
    public async Task<Guid?> Handle(CreateAccountCommand request, CancellationToken cancellationToken = default)
    {
        var currencyExists = await dbContext.Currencies
            .AnyAsync(currency => currency.Code == request.Currency, cancellationToken);
        if (!currencyExists)
        {
            throw new InvalidOperationException($"Currency '{request.Currency}' does not exist.");
        }

        var account = Account.Create(Guid.NewGuid(), request.Currency, request.UserId);

        if (request.InitialAmount <= 0)
        {
            return null;
        }

        var initialMoney = new Money(request.InitialAmount, request.Currency);
        account.Deposit(initialMoney);

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
