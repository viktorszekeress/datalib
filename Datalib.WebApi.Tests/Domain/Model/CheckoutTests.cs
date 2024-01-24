using Datalib.WebApi.Domain.Models;

namespace Datalib.WebApi.Tests.Domain.Model;

[Trait("Category", "Checkout")]
public class CheckoutTests
{
    [Fact(DisplayName = "CreateForBooks returns checkout with assigned user")]
    public void CreateForBooks_AssignsUser()
    {
        // Arrange
        var book = SetupBook();
        var user = SetupUser();

        // Act
        var checkout = Checkout.CreateForBooks(user, book);

        // Assert
        checkout.IssuedToUserId.Should().Be(user.Id);
    }

    [Theory(DisplayName = "CreateForBooks returns checkout with same number of items as books")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(25)]
    public void CreateForBooks_CreatesSameNumberOfItemsAsBooks(int numberOfBooks)
    {
        // Arrange
        var books = Enumerable.Range(0, numberOfBooks).Select(_ => SetupBook()).ToArray();
        var user = SetupUser();

        // Act
        var checkout = Checkout.CreateForBooks(user, books);

        // Assert
        checkout.Items.Should().HaveCount(numberOfBooks);
    }

    [Theory(DisplayName = "CreateForBooks returns checkout with assigned items")]
    [InlineData(1)]
    [InlineData(10)]
    public void CreateForBooks_AssignsItems(int numberOfBooks)
    {
        // Arrange
        var books = Enumerable.Range(0, numberOfBooks).Select(_ => SetupBook()).ToArray();
        var user = SetupUser();

        // Act
        var checkout = Checkout.CreateForBooks(user, books);

        // Assert
        checkout.Items.Select(i => i.CheckoutId).Distinct()
            .Should().Equal(checkout.Id);
        checkout.Items.Select(i => i.Book).Should()
            .HaveCount(books.Length)
            .And.ContainInOrder(books);
    }

    [Fact(DisplayName = "CreateForBooks returns checkout with IssuedOn set to current datetime")]
    public void CreateForBooks_AssignsIssuedOnTime()
    {
        // Arrange
        var user = SetupUser();

        // Act
        var checkout = Checkout.CreateForBooks(user);

        // Assert
        checkout.IssuedOn.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(1));
    }

    [Theory(DisplayName = "CreateForBooks returns checkout with items with correct DueDate")]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void CreateForBooks_CreatesItemsWithCorrectDueDate(int days)
    {
        // Arrange
        Checkout.DefaultCheckoutPeriod = TimeSpan.FromDays(days);
        var expectedDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(days));
        var user = SetupUser();
        var book1 = SetupBook();
        var book2 = SetupBook();

        // Act
        var checkout = Checkout.CreateForBooks(user, book1, book2);

        // Assert
        checkout.Items.Should().AllSatisfy(i => i.DueDate.Should().Be(expectedDueDate));
    }

    private static Book SetupBook() => new() {Id = Guid.NewGuid(), Title = "Book", Author = "Author"};
    
    private static User SetupUser() => new() {Id = Guid.NewGuid(), FullName = "User", Email = "email@abc.com"};
}