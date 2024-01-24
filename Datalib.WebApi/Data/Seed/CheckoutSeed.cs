using Datalib.WebApi.Domain.Models;

namespace Datalib.WebApi.Data.Seed;

public class CheckoutSeed : BaseSeed<Checkout>
{
    public CheckoutSeed(IUnitOfWork dbContext) : base(dbContext)
    {
    }

    protected override Func<IUnitOfWork, IRepository<Checkout>> RepositorySelector => dbContext => dbContext.Checkouts;

    protected override Checkout? TryFindExisting(IRepository<Checkout> repository, Checkout entity)
    {
        return repository.FirstOrDefault(c => c.Id == entity.Id);
    }

    protected override IEnumerable<Checkout> GetSeedData(IUnitOfWork dbContext)
    {
        var books = dbContext.Books.All.ToList();
        var users = dbContext.Users.All.ToList();
        var user1 = users.ElementAtOrDefault(1);
        var user2 = users.ElementAtOrDefault(2);
        var book1 = books.ElementAtOrDefault(0);
        var book2 = books.ElementAtOrDefault(1);
        var book3 = books.ElementAtOrDefault(2);
        var book4 = books.ElementAtOrDefault(3);
        var book5 = books.ElementAtOrDefault(4);

        if (user1 is null || user2 is null || book1 is null || book2 is null || book3 is null || book4 is null || book5 is null)
        {
            return Enumerable.Empty<Checkout>();
        }

        var checkout1 = user1.CheckoutBooks(book1, book2);
        var checkout2 = user1.CheckoutBooks(book3);
        var checkout3 = user2.CheckoutBooks(book4, book5);

        return new[] {checkout1, checkout2, checkout3};
    }
}