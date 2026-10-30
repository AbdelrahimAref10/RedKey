using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Contracts.Persistence;
using RedKey.Domain.Entities;
using RedKey.Domain.Entities.Accounting;
using RedKey.Domain.Entities.Identity;
using RedKey.Domain.Entities.Units;

namespace RedKey.Persistence.Contexts;

public sealed class RedKeyDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    public RedKeyDbContext(DbContextOptions<RedKeyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Broker> Brokers => Set<Broker>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<UnitType> UnitTypes => Set<UnitType>();
    public DbSet<PropertyUnit> Units => Set<PropertyUnit>();
    public DbSet<ViewingRequest> ViewingRequests => Set<ViewingRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();

    public async Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        new DatabaseTransaction(await Database.BeginTransactionAsync(cancellationToken));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity mapping first, then our configurations override it (RK_ table names, indexes...).
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasSequence<int>(Configurations.TableNames.UnitCodeSequence).StartsAt(1000);
        modelBuilder.HasSequence<int>(Configurations.TableNames.JournalNumberSequence).StartsAt(100001);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RedKeyDbContext).Assembly);
    }
}
