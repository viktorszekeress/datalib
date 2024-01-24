using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Datalib.WebApi.Data.Implementation;

public class DbSetRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet;
    private readonly Func<DbSet<TEntity>, IQueryable<TEntity>> _baseQuery;

    public IQueryable<TEntity> All => _baseQuery(_dbSet);

    public DbSetRepository(DbSet<TEntity> dbSet, Func<DbSet<TEntity>, IQueryable<TEntity>> baseQuery)
    {
        _dbSet = dbSet;
        _baseQuery = baseQuery;
    }

    public DbSetRepository(DbSet<TEntity> dbSet)
    {
        _dbSet = dbSet;
        _baseQuery = s => s;
    }

    public TEntity Add(TEntity entity)
    {
        var result = _dbSet.Add(entity);

        return result.Entity;
    }

    public void Remove(TEntity entity) => _dbSet.Remove(entity);

    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> query)
    {
        return _baseQuery(_dbSet).Where(query).ToListAsync();
    }

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> query)
    {
        return _baseQuery(_dbSet).FirstOrDefaultAsync(query);
    }

    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> query)
    {
        return _baseQuery(_dbSet).FirstOrDefault(query);
    }
}