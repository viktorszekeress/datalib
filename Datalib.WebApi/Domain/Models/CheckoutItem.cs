using Datalib.WebApi.Utils;

namespace Datalib.WebApi.Domain.Models;

public class CheckoutItem : BaseModel<Guid>
{
    public Guid CheckoutId { get; set; }
    
    public required Checkout? Checkout { get; init; }
    
    public required Guid BookId { get; set; }
    
    public Book? Book { get; init; }

    public DateOnly DueDate { get; private set; }

    public DateTimeOffset? ReturnedOn { get; private set; }
    
    public CheckoutStatus Status { get; private set; }

    public bool CanCheckOut => Status == CheckoutStatus.Returned;

    public bool CanReturn => Status == CheckoutStatus.CheckedOut;

    public static CheckoutItem CreateCheckedOut(Checkout checkout, Book book, DateOnly dueDate)
    {
        var item = new CheckoutItem
        {
            Id = Guid.Empty,
            Checkout = checkout,
            CheckoutId = checkout.Id,
            Book = book,
            BookId = book.Id,
            DueDate = dueDate,
            Status = CheckoutStatus.CheckedOut
        };
        
        checkout.Items.TryAdd(item);

        return item;
    }

    public void Return()
    {
        Status = CheckoutStatus.Returned;
        ReturnedOn = DateTimeOffset.Now;
    }
}