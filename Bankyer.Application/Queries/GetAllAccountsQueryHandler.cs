using Bankyer.Domain.Aggregates;
using Bankyer.Infrastructure.Database;
using Bankyer.Shared;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Application.Queries;

public class GetAllAccountsQueryHandler(AppDbContext dbContext, IEventStore eventStore)
{
    public async Task<List<GetAllAccountsResponse>> HandleAsync()
    {
        var entities = await dbContext.Accounts.AsNoTracking().ToListAsync();
        var accounts = new List<GetAllAccountsResponse>(entities.Count);

        foreach (var entity in entities)
        {
            var aggregate = new Account();
            aggregate.LoadFromHistory(await eventStore.GetEventsAsync(entity.Id));
            accounts.Add(new GetAllAccountsResponse
            {
                Id = entity.Id,
                Status = entity.Status.ToString(),
                Balance = entity.Balance,
                Currency = aggregate.Balance.Currency.ToString(),
            });
        }

        return accounts;
    }
}
