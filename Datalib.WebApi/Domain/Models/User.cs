namespace Datalib.WebApi.Domain.Models;

public class User : BaseModel<Guid>
{
    public required string FullName { get; set; }

    public required string Email { get; set; }

    public bool IsAdmin { get; set; }
    
    public List<Checkout> Checkouts { get; } = new();

    public Checkout CheckoutBooks(params Book[] books)
    {
        var checkout = Checkout.CreateForBooks(this, books);

        Checkouts.Add(checkout);

        return checkout;
    }

    public void Update(string fullName, string email) => (FullName, Email) = (fullName, email);
}