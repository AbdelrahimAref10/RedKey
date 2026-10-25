using Microsoft.EntityFrameworkCore.Storage;
using RedKey.Application.Contracts.Persistence;

namespace RedKey.Persistence.Contexts;

internal sealed class DatabaseTransaction : IDatabaseTransaction
{
    private readonly IDbContextTransaction _transaction;

    public DatabaseTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        _transaction.CommitAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken = default) =>
        _transaction.RollbackAsync(cancellationToken);

    public ValueTask DisposeAsync() => _transaction.DisposeAsync();
}
