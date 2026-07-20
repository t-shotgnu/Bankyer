using Bankyer.Domain;
using Bankyer.Domain.Aggregates;
using Bankyer.Infrastructure.Database;
using Bankyer.Shared;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Application.Queries;

public class GetAccountQueryHandler(AppDbContext dbContext, IEventStore eventStore)
{
    public async Task<GetAccountResponse?> Handle(GetAccountQuery request, CancellationToken cancellationToken = default)
    {
        var records = await eventStore.GetEventRecordsAsync(request.Id);
        
        var accountEntity = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (accountEntity == null)
        {
            return null;
        }
        
        var account = new Account();
        account.LoadFromHistory(records.Select(record => record.Event));

        var transactions = records
            .Select(record => record.Event switch
            {
                MoneyDepositedEvent deposit => new GetAccountResponse.Transaction
                {
                    Amount = deposit.Amount,
                    Date = record.CreatedAt,
                    Type = GetAccountResponse.TransactionType.Deposit,
                },
                MoneyWithdrawnEvent withdrawal => new GetAccountResponse.Transaction
                {
                    Amount = withdrawal.Amount,
                    Date = record.CreatedAt,
                    Type = GetAccountResponse.TransactionType.Withdrawal,
                },
                _ => null,
            })
            .Where(transaction => transaction != null)
            .Select(transaction => transaction!)
            .ToList();

        return new GetAccountResponse
        {
            Id = request.Id,
            Balance = accountEntity.Balance,
            Currency = account.Balance.Currency.ToString(),
            Status = account.Status.ToString(),
            Transactions = transactions,
        };
    }
}
