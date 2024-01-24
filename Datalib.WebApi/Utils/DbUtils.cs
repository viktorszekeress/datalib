using Datalib.WebApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Datalib.WebApi.Utils;

public static class DbUtils
{
    public static async Task<EntityEntry<TModel>> AddOrUpdateAsync<TModel, TId>(this DbSet<TModel> set, TModel value) where TModel : BaseModel<TId>
    {
        var exists = set.AsNoTracking().Any(o => o.Id != null && o.Id.Equals(value.Id));
        if (exists)
        {
            return set.Update(value);
        }

        return await set.AddAsync(value);
    }
}