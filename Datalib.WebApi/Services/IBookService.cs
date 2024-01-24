using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos.Requests;

namespace Datalib.WebApi.Services;

public interface IBookService
{
    Task<Result<Book>> CreateBookAsync(BookRequest request);

    Task<Result<List<Book>>> GetAllBooksAsync();
    
    Task<Result<Book>> GetBookAsync(Guid id);

    Task<Result> UpdateBookAsync(Guid id, BookRequest request);

    Task<Result> DeleteBookAsync(Guid id);
}