using Datalib.WebApi.Controllers;
using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos.Responses;
using Datalib.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Datalib.WebApi.Tests.Controllers;

[Trait("Category", "CheckoutsController")]
public class CheckoutsControllerTests
{
    private Mock<ILogger<CheckoutsController>> LoggerMock { get; }

    public CheckoutsControllerTests()
    {
        LoggerMock = new Mock<ILogger<CheckoutsController>>();
    }

    [Fact(DisplayName = "GetCheckoutsForUserAsync returns 404 NotFound if user doesn't exist")]
    public async Task GetCheckoutsForUserAsync_Returns404NotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var checkoutServiceMock = new Mock<ICheckoutService>();
        checkoutServiceMock.Setup(s => s.GetCheckoutsForUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Fail<List<Checkout>>("Error"));
        var controller = new CheckoutsController(checkoutServiceMock.Object, LoggerMock.Object);

        // Act
        var actionResult = await controller.GetCheckoutsForUserAsync(Guid.Empty);

        // Assert
        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact(DisplayName = "GetCheckoutsForUserAsync returns 500 InternalServerError when exception is thrown")]
    public async Task GetCheckoutsForUserAsync_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var checkoutServiceMock = new Mock<ICheckoutService>();
        checkoutServiceMock.Setup(s => s.GetCheckoutsForUserAsync(It.IsAny<Guid>())).Throws(new Exception());
        var controller = new CheckoutsController(checkoutServiceMock.Object, LoggerMock.Object);

        // Act
        var actionResult = await controller.GetCheckoutsForUserAsync(Guid.Empty);

        // Assert
        actionResult.Result.Should().BeAssignableTo<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact(DisplayName = "GetCheckoutsForUserAsync returns array of checkout responses when user is found")]
    public async Task GetCheckoutsForUserAsync_ReturnsArrayOfCheckoutResponses_WhenUserIsFound()
    {
        // Arrange
        var book1 = SetupBook();
        var book2 = SetupBook();
        var book3 = SetupBook();
        var user = SetupUser();
        var checkout1 = Checkout.CreateForBooks(user, book1, book2);
        var checkout2 = Checkout.CreateForBooks(user, book3);
        var checkouts = new List<Checkout> {checkout1, checkout2};

        var checkoutServiceMock = new Mock<ICheckoutService>();
        checkoutServiceMock.Setup(s => s.GetCheckoutsForUserAsync(It.Is(user.Id, EqualityComparer<Guid>.Default)))
            .ReturnsAsync(Result.Ok(checkouts));
        var controller = new CheckoutsController(checkoutServiceMock.Object, LoggerMock.Object);

        // Act
        var actionResult = await controller.GetCheckoutsForUserAsync(user.Id);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<CheckoutResponse>>();
    }

    // Similarly, test other actions...

    private static Book SetupBook() => new() {Id = Guid.NewGuid(), Title = "Book", Author = "Author"};

    private static User SetupUser() => new() {Id = Guid.NewGuid(), FullName = "User", Email = "email@abc.com"};
}