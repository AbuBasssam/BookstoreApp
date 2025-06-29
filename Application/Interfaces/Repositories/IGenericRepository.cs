using Domain;

namespace Application.Interfaces;

public interface IGenericRepository<T, TKey> where T : class, IEntity<TKey>
{
    IQueryable<T> GetByIdAsync(TKey id);
    IQueryable<T> GetTableNoTracking();
    IQueryable<T> GetTableAsTracking();
    IQueryable<T> GetPage(int PageNumber = 1);

    Task<bool> IsExistsByIdAsync(TKey id);
    Task<TKey> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<bool> AddRangeAsync(ICollection<T> entities);
    Task<bool> UpdateRangeAsync(ICollection<T> entities);
    Task<bool> DeleteRangeAsync(ICollection<T> entities);
    
}
