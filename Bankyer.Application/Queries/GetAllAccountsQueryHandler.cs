using Bankyer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Application.Queries;

public class GetAllAccountsQueryHandler(AppDbContext dbContext)
{
    public async Task<List<GetAllAccountsResponse>> HandleAsync()
    {
        return await dbContext.Accounts
            .Select(a => new GetAllAccountsResponse
            {
                Id = a.Id,
                Balance = a.Balance,
                Status = a.Status.ToString(),
            })
            .ToListAsync();
    }
}
