using Microsoft.EntityFrameworkCore.Storage;

namespace Pizzashop.Repository.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}