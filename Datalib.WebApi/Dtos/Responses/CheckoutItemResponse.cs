namespace Datalib.WebApi.Dtos.Responses;

public record struct CheckoutItemResponse(BookResponse Book, DateOnly DueDate, DateTimeOffset? ReturnedOn);