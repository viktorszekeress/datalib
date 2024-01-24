using Datalib.WebApi.Domain.Models;

namespace Datalib.WebApi.Tests.Domain.Model;

[Trait("Category", "CheckoutItem")]
public class CheckoutItemTests
{
    [Fact(DisplayName = "CreateCheckedOut returns item with CheckedOut status")]
    public void CreateCheckedOut_ItemStatusIsCheckedOut()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();
        var book = SetupBook();

        // Act
        var item = CheckoutItem.CreateCheckedOut(checkout, book, DateOnly.MaxValue);

        // Assert
        item.Status.Should().Be(CheckoutStatus.CheckedOut);
    }

    [Fact(DisplayName = "CreateCheckedOut assigns book")]
    public void CreateCheckedOut_ItemHasBookAssigned()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();
        var book = SetupBook();

        // Act
        var item = CheckoutItem.CreateCheckedOut(checkout, book, DateOnly.MaxValue);

        // Assert
        item.BookId.Should().Be(book.Id);
    }

    [Fact(DisplayName = "CreateCheckedOut assigns checkout")]
    public void CreateCheckedOut_ItemHasCheckoutAssigned()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();
        var book = SetupBook();

        // Act
        var item = CheckoutItem.CreateCheckedOut(checkout, book, DateOnly.MaxValue);

        // Assert
        item.CheckoutId.Should().Be(checkout.Id);
    }

    [Fact(DisplayName = "CreateCheckedOut assigns due date")]
    public void CreateCheckedOut_ItemHasDueDateAssigned()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();
        var book = SetupBook();
        var date = DateOnly.FromDateTime(new DateTime(2020, 1, 1));

        // Act
        var item = CheckoutItem.CreateCheckedOut(checkout, book, date);

        // Assert
        item.DueDate.Should().Be(date);
    }

    private static Book SetupBook() => new() {Id = Guid.NewGuid(), Title = "Book", Author = "Author"};

    private static Checkout SetupUserAndCheckout()
    {
        var user = new User {Id = Guid.NewGuid(), FullName = "User", Email = "email@abc.com"};

        var checkout = Checkout.CreateForBooks(user);

        return checkout;
    }
}