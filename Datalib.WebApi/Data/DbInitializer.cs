using Datalib.WebApi.Data.Implementation;
using Datalib.WebApi.Data.Seed;
using Datalib.WebApi.Domain.Models;

namespace Datalib.WebApi.Data;

public static class DbInitializer
{
    public static void Initialize(DatalibDbContext dbContext, IServiceProvider services)
    {
        dbContext.Database.EnsureCreated();

        var bookSeed = services.GetRequiredService<ISeed<Book>>();
        bookSeed.Seed();

        var userSeed = services.GetRequiredService<ISeed<User>>();
        userSeed.Seed();

        var checkoutSeed = services.GetRequiredService<ISeed<Checkout>>();
        checkoutSeed.Seed();
    }
}