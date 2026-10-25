using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Contracts.Persistence;
using RedKey.Domain.Entities;
using RedKey.Domain.Entities.Identity;

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

    public async Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        new DatabaseTransaction(await Database.BeginTransactionAsync(cancellationToken));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity mapping first, then our configurations override it (RK_ table names, indexes...).
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RedKeyDbContext).Assembly);
    }
}
