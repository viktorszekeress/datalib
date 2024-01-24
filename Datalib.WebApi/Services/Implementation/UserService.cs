using Datalib.WebApi.Data;
using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos;
using Datalib.WebApi.Dtos.Requests;
using Microsoft.EntityFrameworkCore;

namespace Datalib.WebApi.Services.Implementation;

public class UserService : IUserService
{
    private readonly IUnitOfWork _dbContext;

    public UserService(IUnitOfWork dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<User>> CreateUserAsync(UserRequest request)
    {
        if (await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email) is not null)
        {
            return Result.Fail<User>($"User with Email={request.Email} already exists.");
        }

        var user = request.ToUser();
        _dbContext.Users.Add(user);

        await _dbContext.CommitAsync();

        return Result.Ok(user);
    }

    public async Task<Result<List<User>>> GetAllUsersAsync()
    {
        var users = await _dbContext.Users.All.ToListAsync();

        return Result.Ok(users);
    }

    public async Task<Result<User>> GetUserAsync(Guid id)
    {
        if (await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id) is not { } user)
        {
            return Result.Fail<User>($"User with Id={id} not found.");
        }

        return Result.Ok(user);
    }

    public async Task<Result> UpdateUserAsync(Guid id, UserRequest request)
    {
        if (await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id) is not { } user)
        {
            return Result.Fail<User>($"User with Id={id} not found.");
        }

        user.Update(request.FullName, request.Email);

        await _dbContext.CommitAsync();

        return Result.Ok(user);
    }

    public async Task<Result> DeleteUserAsync(Guid id)
    {
        if (await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id) is not { } user)
        {
            return Result.Fail<User>($"User with Id={id} not found.");
        }

        _dbContext.Users.Remove(user);

        await _dbContext.CommitAsync();

        return Result.Ok();
    }
}