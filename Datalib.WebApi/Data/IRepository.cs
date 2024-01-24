using System.Linq.Expressions;

namespace Datalib.WebApi.Data;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> All { get; }

    TEntity Add(TEntity entity);
    void Remove(TEntity entity);
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> query);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> query);
    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> query);
}