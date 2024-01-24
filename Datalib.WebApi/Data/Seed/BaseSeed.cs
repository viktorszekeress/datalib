using Datalib.WebApi.Utils;

namespace Datalib.WebApi.Data.Seed;

public abstract class BaseSeed<TEntity> : ISeed<TEntity> where TEntity : class
{
    private readonly IUnitOfWork _dbContext;

    protected abstract Func<IUnitOfWork, IRepository<TEntity>> RepositorySelector { get; }

    private IRepository<TEntity> Repository => RepositorySelector(_dbContext);

    protected BaseSeed(IUnitOfWork dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        if (Repository.All.Any())
        {
            return;
        }

        var seedData = GetSeedData(_dbContext);
        seedData.ForEach(entity => EnsureEqualExists(entity));
    }

    public TEntity EnsureEqualExists(TEntity entity)
    {
        if (TryFindExisting(Repository, entity) is { } existing)
        {
            return existing;
        }

        Repository.Add(entity);
        _dbContext.Commit();

        return entity;
    }

    protected abstract TEntity? TryFindExisting(IRepository<TEntity> repository, TEntity entity);

    protected abstract IEnumerable<TEntity> GetSeedData(IUnitOfWork dbContext);
}