using Bankyer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Application.Commands.DeleteAccount;

public class DeleteAccountCommandHandler(AppDbContext dbContext)
{
    public async Task<bool> Handle(DeleteAccountCommand request, string userId, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Accounts
            .FirstOrDefaultAsync(account => account.Id == request.Id && account.UserId == userId, cancellationToken);
        if (entity == null) return false;

        dbContext.Accounts.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
