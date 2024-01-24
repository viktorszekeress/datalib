namespace Datalib.WebApi.Data.Seed;

public class NoSeed<TEntity> : ISeed<TEntity>
{
    public void Seed()
    {
    }

    public TEntity EnsureEqualExists(TEntity entity) => entity;
}