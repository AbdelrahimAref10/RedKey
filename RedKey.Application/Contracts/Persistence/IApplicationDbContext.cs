using Microsoft.EntityFrameworkCore;
using RedKey.Domain.Entities;
using RedKey.Domain.Entities.Accounting;
using RedKey.Domain.Entities.Identity;
using RedKey.Domain.Entities.Units;

namespace RedKey.Application.Contracts.Persistence;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Broker> Brokers { get; }
    DbSet<City> Cities { get; }
    DbSet<ContactMessage> ContactMessages { get; }
    DbSet<Favorite> Favorites { get; }
    DbSet<Area> Areas { get; }
    DbSet<UnitType> UnitTypes { get; }
    DbSet<PropertyUnit> Units { get; }
    DbSet<ViewingRequest> ViewingRequests { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Account> Accounts { get; }
    DbSet<JournalEntry> JournalEntries { get; }
    DbSet<JournalLine> JournalLines { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
