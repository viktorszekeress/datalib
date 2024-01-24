using Datalib.WebApi.Domain.Models;

namespace Datalib.WebApi.Data.Seed;

public class UserSeed : BaseSeed<User>
{
    protected override Func<IUnitOfWork, IRepository<User>> RepositorySelector => dbContext => dbContext.Users;

    public UserSeed(IUnitOfWork dbContext) : base(dbContext)
    {
    }

    protected override User? TryFindExisting(IRepository<User> repository, User entity)
    {
        return repository.FirstOrDefault(u => u.FullName == entity.FullName);
    }

    protected override IEnumerable<User> GetSeedData(IUnitOfWork dbContext)
    {
        return new User[]
        {
            new() {Id = Guid.Empty, FullName = "Admin", Email = "abc0@def.com", IsAdmin = true},
            new() {Id = Guid.Empty, FullName = "User1", Email = "abc1@def.com"},
            new() {Id = Guid.Empty, FullName = "User2", Email = "abc2@def.com"},
            new() {Id = Guid.Empty, FullName = "User3", Email = "abc3@def.com"}
        };
    }
}