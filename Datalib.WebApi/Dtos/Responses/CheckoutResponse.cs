namespace Datalib.WebApi.Dtos.Responses;

public record struct CheckoutResponse(Guid Id, IEnumerable<CheckoutItemResponse> Items, Guid IssuedToUserId, DateTimeOffset IssuedOn);