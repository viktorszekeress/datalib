using Datalib.WebApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Datalib.WebApi.Data.Implementation;

public class DatalibDbContext : DbContext, IUnitOfWork
{
    public DatalibDbContext(DbContextOptions<DatalibDbContext> options) : base(options)
    {
    }

    public DbSet<Book> BooksDbSet => base.Set<Book>();
    public DbSet<Checkout> CheckoutsDbSet => base.Set<Checkout>();
    public DbSet<CheckoutItem> CheckoutItemsDbSet => base.Set<CheckoutItem>();
    public DbSet<User> UsersDbSet => base.Set<User>();

    public IRepository<Book> Books => new DbSetRepository<Book>(BooksDbSet);
    public IRepository<Checkout> Checkouts => new DbSetRepository<Checkout>(CheckoutsDbSet, dbSet => dbSet.Include(c => c.Items).ThenInclude(i => i.Book));
    public IRepository<CheckoutItem> CheckoutItems => new DbSetRepository<CheckoutItem>(CheckoutItemsDbSet);
    public IRepository<User> Users => new DbSetRepository<User>(UsersDbSet);

    public void Commit() => SaveChanges();
    
    public Task CommitAsync() => SaveChangesAsync();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Prefer singular over plural table names.
        modelBuilder.Entity<Book>().ToTable(nameof(Book));
        modelBuilder.Entity<Checkout>().ToTable(nameof(Checkout));
        modelBuilder.Entity<CheckoutItem>().ToTable(nameof(CheckoutItem));
        modelBuilder.Entity<User>().ToTable(nameof(User));
    }
}