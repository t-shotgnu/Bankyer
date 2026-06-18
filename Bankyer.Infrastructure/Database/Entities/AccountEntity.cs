using Bankyer.Domain.ValueObjects;

namespace Bankyer.Infrastructure.Database.Entities;

public class AccountEntity
{
    public Guid Id { get; set; }
    public AccountStatus Status { get; set; }
}
