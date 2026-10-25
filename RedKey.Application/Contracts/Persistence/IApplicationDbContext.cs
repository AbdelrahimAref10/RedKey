using Microsoft.EntityFrameworkCore;
using RedKey.Domain.Entities;

namespace RedKey.Application.Contracts.Persistence;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Broker> Brokers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
