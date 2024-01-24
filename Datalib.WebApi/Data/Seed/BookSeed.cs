using Datalib.WebApi.Domain.Models;

namespace Datalib.WebApi.Data.Seed;

public class BookSeed : BaseSeed<Book>
{
    protected override Func<IUnitOfWork, IRepository<Book>> RepositorySelector => dbContext => dbContext.Books;

    public BookSeed(IUnitOfWork dbContext) : base(dbContext)
    {
    }

    protected override Book? TryFindExisting(IRepository<Book> repository, Book entity)
    {
        return repository.FirstOrDefault(b => b.Title == entity.Title);
    }

    protected override IEnumerable<Book> GetSeedData(IUnitOfWork dbContext)
    {
        return new Book[]
        {
            new() {Id = Guid.NewGuid(), Author = "Author1", Title = "Book1"},
            new() {Id = Guid.NewGuid(), Author = "Author2", Title = "Book2"},
            new() {Id = Guid.NewGuid(), Author = "Author3", Title = "Book3"},
            new() {Id = Guid.NewGuid(), Author = "Author4", Title = "Book4"},
            new() {Id = Guid.NewGuid(), Author = "Author5", Title = "Book5"},
            new() {Id = Guid.NewGuid(), Author = "Author6", Title = "Book6"},
            new() {Id = Guid.NewGuid(), Author = "Author7", Title = "Book7"},
            new() {Id = Guid.NewGuid(), Author = "Author8", Title = "Book8"},
            new() {Id = Guid.NewGuid(), Author = "Author9", Title = "Book9"}
        };
    }
}