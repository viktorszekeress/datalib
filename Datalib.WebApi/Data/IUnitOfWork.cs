using Datalib.WebApi.Domain.Models;

namespace Datalib.WebApi.Data;

public interface IUnitOfWork : IDisposable
{
    public IRepository<Book> Books { get; }
    public IRepository<Checkout> Checkouts { get; }
    public IRepository<CheckoutItem> CheckoutItems { get; }
    public IRepository<User> Users { get; }

    void Commit();
    Task CommitAsync();
}