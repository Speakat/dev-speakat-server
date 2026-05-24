using Microsoft.EntityFrameworkCore.Storage;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync()
        => _transaction = await db.Database.BeginTransactionAsync();

    public async Task CommitAsync()
    {
        await _transaction!.CommitAsync();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        await _transaction!.RollbackAsync();
        _transaction = null;
    }
}
