namespace Datalib.WebApi.Dtos.Requests;

public record struct CreateCheckoutRequest(Guid UserId, IEnumerable<Guid> BookIds);
