using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bankyer.Infrastructure.Database.Entities;

[Table("Currencies")]
public class CurrencyEntity
{
    public required Guid Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required, MaxLength(3)]
    public required string Code { get; set; }

    public ICollection<AccountEntity> Accounts { get; set; } = [];
}
