using Datalib.WebApi.Data;
using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos.Requests;

namespace Datalib.WebApi.Services.Implementation;

public class CheckoutService : ICheckoutService
{
    private readonly IUnitOfWork _dbContext;

    public CheckoutService(IUnitOfWork dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Checkout>> CheckOutBooksAsync(CreateCheckoutRequest request)
    {
        if (!request.BookIds.Any())
        {
            return Result.Fail<Checkout>("Book ids list must not be empty.");
        }

        if (await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId) is not {} user)
        {
            return Result.Fail<Checkout>($"User with Id={request.UserId} not found.");
        }

        var books = new List<Book>();
        foreach (var bookId in request.BookIds)
        {
            if (await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId) is not { } book)
            {
                return Result.Fail<Checkout>($"Book with Id={bookId} not found.");
            }

            var allCheckoutItemsForBook = await _dbContext.CheckoutItems.WhereAsync(i => i.BookId == book.Id);
            if (allCheckoutItemsForBook.Any(i => !i.CanCheckOut))
            {
                return Result.Fail<Checkout>($"Cannot check out Book with Id={book.Id}, it is currently checked out.");
            }

            books.Add(book);
        }

        var checkout = user.CheckoutBooks(books.ToArray());
        await _dbContext.CommitAsync();

        return Result.Ok(checkout);
    }

    public async Task<Result<List<Checkout>>> GetCheckoutsForUserAsync(Guid userId)
    {
        if (await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId) is not { } user)
        {
            return Result.Fail<List<Checkout>>($"User with Id={userId} not found.");
        }

        var checkouts = await _dbContext.Checkouts.WhereAsync(c => c.IssuedToUserId == user.Id);

        return Result.Ok(checkouts);
    }

    public async Task<Result<Checkout>> GetCheckoutAsync(Guid id)
    {
        if (await _dbContext.Checkouts.FirstOrDefaultAsync(c => c.Id == id) is not {} checkout)
        {
            return Result.Fail<Checkout>($"Checkout with Id={id} not found.");
        }

        return Result.Ok(checkout);
    }

    public async Task<Result> ReturnBooksAsync(Guid checkoutId, ReturnBooksRequest request)
    {
        if (!request.BookIds.Any())
        {
            return Result.Fail("Book ids list must not be empty.");
        }

        if (await _dbContext.Checkouts.FirstOrDefaultAsync(c => c.Id == checkoutId) is not { } checkout)
        {
            return Result.Fail($"Checkout with Id={checkoutId} not found.");
        }

        if (await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId) is not { } user)
        {
            return Result.Fail($"The specified user with Id={request.UserId} was not found.");
        }

        if (checkout.IssuedToUserId != user.Id)
        {
            return Result.Fail($"This checkout was not issued to the user with Id={user.Id}");
        }

        var allCheckoutItems = await _dbContext.CheckoutItems.WhereAsync(i => i.CheckoutId == checkoutId);

        foreach (var bookId in request.BookIds)
        {
            if (await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId) is not { } book)
            {
                return Result.Fail($"Book with Id={bookId} not found.");
            }

            if (allCheckoutItems.FirstOrDefault(i => i.BookId == bookId) is not { } checkoutItem)
            {
                return Result.Fail($"Book with Id={bookId} not found on the checkout.");
            }

            if (!checkoutItem.CanReturn)
            {
                return Result.Fail($"Cannot return Book with Id={book.Id}, it is already returned.");
            }

            checkoutItem.Return();
        }

        await _dbContext.CommitAsync();

        return Result.Ok();
    }

    public async Task<List<ReminderInfo>> GetItemToRemindAsync()
    {
        var intervalInDays = 20; // Just for testing, otherwise use:
        //var intervalInDays = Checkout.DefaultCheckoutPeriod;
        var targetDate = DateOnly.FromDateTime(DateTime.Now).AddDays(intervalInDays);

        var itemsToRemind = await _dbContext.CheckoutItems
            .WhereAsync(i => i.Status == CheckoutStatus.CheckedOut && i.ReturnedOn == null && i.DueDate < targetDate);

        var groups = itemsToRemind.GroupBy(i => i.CheckoutId);

        var result = new List<ReminderInfo>();
        foreach (var group in groups)
        {
            var info = new ReminderInfo {AuthorsAndTitles = new List<string>()};

            // Checkout, User and Book are not expanded by default, load them from db.
            var checkout = await _dbContext.Checkouts.FirstOrDefaultAsync(c => c.Id == group.Key);
            if (checkout is null)
            {
                continue;
            }

            info.IssuedOn = checkout.IssuedOn;

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == checkout.IssuedToUserId);
            if (user is null)
            {
                continue;
            }

            info.Email = user.Email;

            foreach (var item in group)
            {
                var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == item.BookId);
                if (book is null)
                {
                    continue;
                }
                
                info.AuthorsAndTitles.Add($"{book.Author}: {book.Title}");
            }

            result.Add(info);
        }

        return result;
    }
}