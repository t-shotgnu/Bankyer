using System.ComponentModel.DataAnnotations;

namespace Bankyer.Infrastructure.Database.Entities;

public class AccountEntity
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public required string Status { get; set; }
    public required decimal Balance { get; set; }
    [Required]
    [MaxLength(3)]
    public required string CurrencyCode { get; set; }
    [Required]
    public required CurrencyEntity Currency { get; set; } = null!;
    public string? UserId { get; set; }
    public Microsoft.AspNetCore.Identity.IdentityUser? User { get; set; }
}
