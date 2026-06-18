using Bankyer.Infrastructure.Database;

namespace Bankyer.Application.Commands.DeleteAccount;

public class DeleteAccountCommandHandler(AppDbContext dbContext)
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Accounts.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        dbContext.Accounts.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}

