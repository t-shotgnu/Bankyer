using Bankyer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Application.Queries;

public class GetAllAccountsQueryHandler(AppDbContext dbContext)
{
    public async Task<List<GetAllAccountsResponse>> HandleAsync(string userId)
    {
        return await dbContext.Accounts
            .Where(account => account.UserId == userId)
            .Select(a => new GetAllAccountsResponse
            {
                Id = a.Id,
                Balance = a.Balance,
                Status = a.Status.ToString(),
                Currency = new GetAllAccountsResponse.GetCurrencyResponse
                {
                    Name = a.Currency.Name,
                    Code = a.Currency.Code
                }
            })
            .ToListAsync();
    }
}
