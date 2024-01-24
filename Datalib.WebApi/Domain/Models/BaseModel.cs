namespace Datalib.WebApi.Domain.Models;

public abstract class BaseModel<TId>
{
    public required TId Id { get; init; }
}