namespace Datalib.WebApi.Data.Seed;

public interface ISeed<TEntity>
{
    public void Seed();

    public TEntity EnsureEqualExists(TEntity entity);

    public IEnumerable<TEntity> EnsureEqualExists(IEnumerable<TEntity> entities)
    {
        return entities.Select(EnsureEqualExists).ToList();
    }
}