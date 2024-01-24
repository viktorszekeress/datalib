namespace Datalib.WebApi.Dtos.Responses;

public record struct UserResponse(Guid Id, string FullName, string Email);