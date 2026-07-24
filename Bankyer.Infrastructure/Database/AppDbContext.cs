using Bankyer.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Infrastructure.Database;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<CurrencyEntity> Currencies { get; set; }
    public DbSet<EventStoreItem> EventStore { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CurrencyEntity>(entity =>
        {
            entity.HasIndex(currency => currency.Code).IsUnique();
            entity.HasData(
                new { Id = Guid.Parse("0f0f9e13-1888-4c2a-9d09-4c2ae7e1a101"), Code = "USD", Name = "US Dollar" },
                new { Id = Guid.Parse("e1c5cc10-5d74-4c25-8b12-c7df83a9e102"), Code = "EUR", Name = "Euro" },
                new { Id = Guid.Parse("cbf50718-21e8-49c4-a6f2-f1a081e2a103"), Code = "GBP", Name = "British Pound" },
                new { Id = Guid.Parse("a8b280f7-e6d9-4f26-8cc4-3dd56e8ca104"), Code = "JPY", Name = "Japanese Yen" },
                new { Id = Guid.Parse("9a7db23d-b9fe-414f-ae45-6054f6cda105"), Code = "AUD", Name = "Australian Dollar" },
                new { Id = Guid.Parse("dbb4a1e0-b5b1-4cc6-855f-6f66119ca106"), Code = "CAD", Name = "Canadian Dollar" },
                new { Id = Guid.Parse("7544f532-fd53-4d33-99ec-a1d7d952a107"), Code = "CHF", Name = "Swiss Franc" },
                new { Id = Guid.Parse("16b0d13c-75bb-414a-b04d-3104f145a108"), Code = "CNY", Name = "Chinese Yuan" },
                new { Id = Guid.Parse("6911b4b3-0d52-43c4-9742-12fe8518a109"), Code = "SEK", Name = "Swedish Krona" },
                new { Id = Guid.Parse("079a340e-4598-4fd4-bf9a-07c79749a110"), Code = "NZD", Name = "New Zealand Dollar" });
        });

        modelBuilder.Entity<AccountEntity>(entity =>
        {
            entity.HasOne(account => account.Currency)
                .WithMany(currency => currency.Accounts)
                .HasForeignKey(account => account.CurrencyCode)
                .HasPrincipalKey(currency => currency.Code)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(account => account.User)
                .WithMany()
                .HasForeignKey(account => account.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(account => account.UserId);
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
