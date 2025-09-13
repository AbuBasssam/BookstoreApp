using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task Commit();
    Task RollBack();
}