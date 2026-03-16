using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mistruna.Core.Contracts.Base.Infrastructure;
using IDbTransaction = Mistruna.Core.Contracts.Base.Infrastructure.IDbTransaction;

namespace Mistruna.Core.Base.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// Wraps <see cref="DbContext"/> to provide transactional boundaries.
/// </summary>
/// <typeparam name="TContext">The DbContext type.</typeparam>
public class EfUnitOfWork<TContext>(TContext context) : IUnitOfWork
    where TContext : DbContext
{
    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new EfDbTransaction(transaction);
    }

    /// <inheritdoc />
    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await action(cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action(cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Wraps EF Core's <see cref="IDbContextTransaction"/> to implement <see cref="IDbTransaction"/>.
/// </summary>
internal sealed class EfDbTransaction(IDbContextTransaction transaction) : IDbTransaction
{
    /// <inheritdoc />
    public Guid TransactionId { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
        => await transaction.CommitAsync(cancellationToken);

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
        => await transaction.RollbackAsync(cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        transaction.Dispose();
    }
}
