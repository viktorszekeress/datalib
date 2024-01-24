namespace Datalib.WebApi.Dtos.Requests;

public record struct ReturnBooksRequest(Guid UserId, IEnumerable<Guid> BookIds);