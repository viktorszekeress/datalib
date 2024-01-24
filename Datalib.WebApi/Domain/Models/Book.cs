namespace Datalib.WebApi.Domain.Models;

public class Book : BaseModel<Guid>
{
    public required string Author { get; set; }

    public required string Title { get; set; }
    
    public List<CheckoutItem> CheckoutItems { get; } = new();

    public void Update(string author, string title) => (Author, Title) = (author, title);
}