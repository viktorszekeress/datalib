using Datalib.WebApi.Data;
using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Dtos;
using Datalib.WebApi.Dtos.Requests;
using Microsoft.EntityFrameworkCore;

namespace Datalib.WebApi.Services.Implementation;

public class BookService : IBookService
{
    private readonly IUnitOfWork _dbContext;

    public BookService(IUnitOfWork dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Book>> CreateBookAsync(BookRequest request)
    {
        var book = request.ToBook();
        _dbContext.Books.Add(book);

        await _dbContext.CommitAsync();

        return Result.Ok(book);
    }

    public async Task<Result<List<Book>>> GetAllBooksAsync()
    {
        var books = await _dbContext.Books.All.OrderBy(b => b.Title).ToListAsync();

        return Result.Ok(books);
    }

    public async Task<Result<Book>> GetBookAsync(Guid id)
    {
        if (await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id) is not { } book)
        {
            return Result.Fail<Book>($"Book with Id={id} not found.");
        }

        return Result.Ok(book);
    }

    public async Task<Result> UpdateBookAsync(Guid id, BookRequest request)
    {
        if (await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id) is not { } book)
        {
            return Result.Fail<Book>($"Book with Id={id} not found.");
        }

        book.Update(request.Author, request.Title);

        await _dbContext.CommitAsync();

        return Result.Ok(book);
    }

    public async Task<Result> DeleteBookAsync(Guid id)
    {
        if (await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id) is not { } book)
        {
            return Result.Fail<Book>($"Book with Id={id} not found.");
        }

        _dbContext.Books.Remove(book);

        await _dbContext.CommitAsync();

        return Result.Ok();
    }
}