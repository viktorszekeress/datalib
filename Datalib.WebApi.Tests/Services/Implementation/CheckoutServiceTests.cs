using System.Linq.Expressions;
using Datalib.WebApi.Data;
using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos.Requests;
using Datalib.WebApi.Services.Implementation;
using Moq;

namespace Datalib.WebApi.Tests.Services.Implementation;

[Trait("Category", "CheckoutService")]
public class CheckoutServiceTests
{
    [Fact(DisplayName = "ReturnBooks fails when list of book ids is empty")]
    public async Task ReturnBooks_Fails_WhenBookIdsListIsEmpty()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();

        var dbContextMock = SetupDbContext(checkout);
        var service = new CheckoutService(dbContextMock.Object);

        // Act
        var result = await service.ReturnBooksAsync(Guid.NewGuid(), new ReturnBooksRequest {BookIds = new List<Guid>()});

        // Assert
        result.CheckSuccess().Should().BeFalse();
    }

    [Fact(DisplayName = "ReturnBooks fails when checkout is not found")]
    public async Task ReturnBooks_Fails_WhenCheckoutIsNotFound()
    {
        // Arrange
        var dbContextMock = SetupDbContext();
        var service = new CheckoutService(dbContextMock.Object);
        var request = new ReturnBooksRequest {BookIds = new List<Guid> {Guid.NewGuid(), Guid.NewGuid()}};

        // Act
        var result = await service.ReturnBooksAsync(Guid.NewGuid(), request);

        // Assert
        result.CheckSuccess().Should().BeFalse();
    }

    [Fact(DisplayName = "ReturnBooks fails when specified user was not found")]
    public async Task ReturnBooks_Fails_WhenUserWasNotFound()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();

        var dbContextMock = SetupDbContext(checkout);
        var service = new CheckoutService(dbContextMock.Object);
        var request = new ReturnBooksRequest {UserId = Guid.NewGuid(), BookIds = new List<Guid> {Guid.NewGuid(), Guid.NewGuid()}};

        // Act
        var result = await service.ReturnBooksAsync(Guid.NewGuid(), request);

        // Assert
        result.CheckSuccess().Should().BeFalse();
    }

    [Fact(DisplayName = "ReturnBooks fails when checkout wasn't issued to specified user")]
    public async Task ReturnBooks_Fails_WhenCheckoutWasNotIssuedToSpecifiedUser()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();
        var otherUser = new User {Id = Guid.NewGuid(), FullName = "user", Email = "email@abc.com"};
        
        var dbContextMock = SetupDbContext(checkout, otherUser);
        var service = new CheckoutService(dbContextMock.Object);
        var request = new ReturnBooksRequest {UserId = Guid.NewGuid(), BookIds = new List<Guid> {Guid.NewGuid(), Guid.NewGuid()}};

        // Act
        var result = await service.ReturnBooksAsync(Guid.NewGuid(), request);

        // Assert
        result.CheckSuccess().Should().BeFalse();
    }

    [Fact(DisplayName = "ReturnBooks fails when book wasn't found")]
    public async Task ReturnBooks_Fails_WhenBookWasNotFound()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();

        var dbContextMock = SetupDbContext(checkout, checkout.IssuedToUser, checkout.Items);
        var service = new CheckoutService(dbContextMock.Object);
        var request = new ReturnBooksRequest {UserId = Guid.NewGuid(), BookIds = new List<Guid> {Guid.NewGuid(), Guid.NewGuid()}};

        // Act
        var result = await service.ReturnBooksAsync(Guid.NewGuid(), request);

        // Assert
        result.CheckSuccess().Should().BeFalse();
    }

    [Fact(DisplayName = "ReturnBooks fails when checkout doesn't contain specified book")]
    public async Task ReturnBooks_Fails_WhenCheckoutDoesNotContainSpecifiedBook()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();
        var otherBook = SetupBook();

        var dbContextMock = SetupDbContext(checkout, checkout.IssuedToUser, checkout.Items, otherBook);
        var service = new CheckoutService(dbContextMock.Object);
        var request = new ReturnBooksRequest {UserId = Guid.NewGuid(), BookIds = new List<Guid> {Guid.NewGuid(), Guid.NewGuid()}};

        // Act
        var result = await service.ReturnBooksAsync(Guid.NewGuid(), request);

        // Assert
        result.CheckSuccess().Should().BeFalse();
    }

    [Fact(DisplayName = "ReturnBooks fails when book is already returned")]
    public async Task ReturnBooks_Fails_WhenBookIsAlreadyReturned()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();
        var item = checkout.Items.First();
        var book = item.Book;
        item.Return();

        var dbContextMock = SetupDbContext(checkout, checkout.IssuedToUser, checkout.Items, book);
        var service = new CheckoutService(dbContextMock.Object);
        var request = new ReturnBooksRequest {UserId = Guid.NewGuid(), BookIds = new List<Guid> {item.BookId}};

        // Act
        var result = await service.ReturnBooksAsync(Guid.NewGuid(), request);

        // Assert
        result.CheckSuccess().Should().BeFalse();
    }

    [Fact(DisplayName = "ReturnBooks succeeds when book is not already returned")]
    public async Task ReturnBooks_Fails_WhenBookIsNotAlreadyReturned()
    {
        // Arrange
        var checkout = SetupUserAndCheckout();
        var item = checkout.Items.First();
        var book = item.Book;

        var dbContext = SetupDbContext(checkout, checkout.IssuedToUser, checkout.Items, book);
        var service = new CheckoutService(dbContext.Object);
        var request = new ReturnBooksRequest {UserId = Guid.NewGuid(), BookIds = new List<Guid> {item.BookId}};

        // Act
        var result = await service.ReturnBooksAsync(Guid.NewGuid(), request);

        // Assert
        result.CheckSuccess().Should().BeTrue();
    }

    private static Mock<IUnitOfWork> SetupDbContext(
        Checkout? checkoutToReturn = null, 
        User? userToReturn = null, 
        List<CheckoutItem>? itemsToReturn = null, 
        Book? bookToReturn = null)
    {
        var dbContextMock = new Mock<IUnitOfWork>();
        var checkoutsRepoMock = new Mock<IRepository<Checkout>>();
        dbContextMock.Setup(c => c.Checkouts).Returns(checkoutsRepoMock.Object);
        if (checkoutToReturn is not null)
        {
            checkoutsRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Checkout, bool>>>())).ReturnsAsync(checkoutToReturn);
        }

        var usersRepoMock = new Mock<IRepository<User>>();
        dbContextMock.Setup(c => c.Users).Returns(usersRepoMock.Object);
        if (userToReturn is not null)
        {
            usersRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(userToReturn);
        }

        var itemsRepoMock = new Mock<IRepository<CheckoutItem>>();
        dbContextMock.Setup(c => c.CheckoutItems).Returns(itemsRepoMock.Object);
        if (itemsToReturn is not null)
        {
            itemsRepoMock.Setup(r => r.WhereAsync(It.IsAny<Expression<Func<CheckoutItem, bool>>>())).ReturnsAsync(itemsToReturn);
        }

        var booksRepoMock = new Mock<IRepository<Book>>();
        dbContextMock.Setup(c => c.Books).Returns(booksRepoMock.Object);
        if (bookToReturn is not null)
        {
            booksRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Book, bool>>>())).ReturnsAsync(bookToReturn);
        }

        return dbContextMock;
    }

    private static Book SetupBook() => new() {Id = Guid.NewGuid(), Author = "Author", Title = "Title"};

    private static Checkout SetupUserAndCheckout()
    {
        var book = SetupBook();
        var user = new User {Id = Guid.NewGuid(), FullName = "User", Email = "email@abc.com"};

        var checkout = Checkout.CreateForBooks(user, book);

        return checkout;
    }
}