namespace Datalib.WebApi.Dtos.Responses;

public record struct BookResponse(Guid Id, string Author, string Title);