using Microsoft.VisualBasic;

namespace Datalib.WebApi.Domain.Models;

public enum CheckoutStatus
{
    Returned,
    CheckedOut
}

public class Checkout : BaseModel<Guid>
{
    public static TimeSpan DefaultCheckoutPeriod = TimeSpan.FromDays(14);
    
    public const ushort RemindIntervalInDays = 1;

    public required Guid IssuedToUserId { get; init; }
    
    public User? IssuedToUser { get; private set; }

    public required DateTimeOffset IssuedOn { get; init; }

    public List<CheckoutItem> Items { get; } = new();

    public static Checkout CreateForBooks(User user, params Book[] books)
    {
        var issuedOn = DateTimeOffset.Now;
        var dueDate = DateOnly.FromDateTime(issuedOn.Add(DefaultCheckoutPeriod).Date);

        var checkout = new Checkout {Id = Guid.Empty, IssuedToUser = user, IssuedToUserId = user.Id, IssuedOn = issuedOn};

        foreach (var book in books)
        {
            CheckoutItem.CreateCheckedOut(checkout, book, dueDate);
        }

        return checkout;
    }

    public IEnumerable<CheckoutItem> GetExpiredItems(ushort remindInterval = RemindIntervalInDays)
    {
        foreach (var checkoutItem in Items.Where(i => i is {Status: CheckoutStatus.CheckedOut, ReturnedOn: null}))
        {
            var dueDt = new DateTimeOffset(checkoutItem.DueDate, TimeOnly.MinValue, DateTimeOffset.Now.Offset);

            var remainingTime = dueDt - DateTimeOffset.Now;

            if (remainingTime.TotalDays < remindInterval)
            {
                yield return checkoutItem;
            }
        }
    }
}