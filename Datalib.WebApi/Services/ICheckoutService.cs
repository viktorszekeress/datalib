using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos.Requests;

namespace Datalib.WebApi.Services;

public interface ICheckoutService
{
    Task<Result<Checkout>> CheckOutBooksAsync(CreateCheckoutRequest request);

    Task<Result<List<Checkout>>> GetCheckoutsForUserAsync(Guid userId);

    Task<Result<Checkout>> GetCheckoutAsync(Guid id);

    Task<Result> ReturnBooksAsync(Guid checkoutId, ReturnBooksRequest request);

    Task<List<ReminderInfo>> GetItemToRemindAsync();
}