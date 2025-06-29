using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Repositories;
public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class, IEntity<TKey>
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> GetByIdAsync(TKey id)
    {

        return _dbSet.Where(entity => entity.Id.Equals(id));
    }

    public IQueryable<T> GetTableNoTracking()
    {
        return _dbSet.AsNoTracking();
    }

    public IQueryable<T> GetTableAsTracking()
    {
        return _dbSet.AsTracking();
    }

    public async Task<bool> IsExistsByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id) != null;
    }

    public async Task<TKey> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return true;
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return true;
    }

    public async Task<bool> AddRangeAsync(ICollection<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return true;
    }

    public async Task<bool> UpdateRangeAsync(ICollection<T> entities)
    {
        _dbSet.UpdateRange(entities);
        return true;
    }

    public async Task<bool> DeleteRangeAsync(ICollection<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return true;
    }



    public virtual IQueryable<T> GetPage(int PageNumber = 1)
                 => _dbSet.AsNoTracking().OrderBy(x => EF.Property<int>(x, "Id")).Skip((PageNumber - 1) * 10).Take(10);

}
