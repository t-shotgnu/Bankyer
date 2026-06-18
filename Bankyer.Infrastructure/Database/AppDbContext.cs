using Bankyer.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AccountEntity> Accounts { get; set; }
    
    public DbSet<EventStoreItem> EventStore { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
