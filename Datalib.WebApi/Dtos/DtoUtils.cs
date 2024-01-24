using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos.Requests;
using Datalib.WebApi.Dtos.Responses;

namespace Datalib.WebApi.Dtos;

public static class DtoUtils
{
    public static Book ToBook(this BookRequest request) => new() {Id = Guid.Empty, Author = request.Author, Title = request.Title};

    public static BookResponse ToBookResponse(this Book book) => new() {Id = book.Id, Author = book.Author, Title = book.Title};

    public static CheckoutItemResponse ToCheckoutItemResponse(this CheckoutItem item)
    {
        return new CheckoutItemResponse
        {
            Book = item.Book?.ToBookResponse() ?? new BookResponse(),
            DueDate = item.DueDate,
            ReturnedOn = item.ReturnedOn
        };
    }

    public static CheckoutResponse ToCheckoutResponse(this Checkout checkout)
    {
        return new()
        {
            Id = checkout.Id,
            Items = checkout.Items.Select(i => i.ToCheckoutItemResponse()),
            IssuedToUserId = checkout.IssuedToUserId,
            IssuedOn = checkout.IssuedOn
        };
    }

    public static User ToUser(this UserRequest request)
    {
        return new User {Id = Guid.Empty, FullName = request.FullName, Email = request.Email};
    }

    public static UserResponse ToUserResponse(this User user) => new() {Id = user.Id, FullName = user.FullName, Email = user.Email};
}