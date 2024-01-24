using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos.Requests;

namespace Datalib.WebApi.Services;

public interface IUserService
{
    Task<Result<User>> CreateUserAsync(UserRequest request);

    Task<Result<List<User>>> GetAllUsersAsync();

    Task<Result<User>> GetUserAsync(Guid id);

    Task<Result> UpdateUserAsync(Guid id, UserRequest request);

    Task<Result> DeleteUserAsync(Guid id);
}