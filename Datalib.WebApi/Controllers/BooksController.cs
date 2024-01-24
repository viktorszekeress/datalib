using System.Net.Mime;
using Datalib.WebApi.Dtos;
using Datalib.WebApi.Dtos.Requests;
using Datalib.WebApi.Dtos.Responses;
using Datalib.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datalib.WebApi.Controllers;

[Route("api/[controller]")]
public class BooksController : BaseApiController
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService, ILogger<BooksController> logger) : base(logger)
    {
        _bookService = bookService;
    }

    // POST api/<BooksController>
    /// <summary>Creates a new book.</summary>
    /// <param name="request">Book to create.</param>
    /// <response code="201">JSON of the created book and location header.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<BookResponse>> CreateAsync([FromBody] BookRequest request)
    {
        try
        {
            var result = await _bookService.CreateBookAsync(request);
            if (!result.CheckSuccess(out var book))
            {
                return BadRequest(new {result.Error});
            }

            var response = book.ToBookResponse();

            return CreatedAtAction("Get", new { id = response.Id }, response);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, "Error creating Book.");
        }
    }

    // GET api/<BooksController>
    /// <summary>Gets all books.</summary>
    /// <response code="200">JSON array of all books.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetAsync()
    {
        try
        {
            var result = await _bookService.GetAllBooksAsync();
            if (!result.CheckSuccess(out var books))
            {
                return BadRequest(new {result.Error});
            }

            var responses = books.Select(b => b.ToBookResponse());

            return Ok(responses);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, "Error getting Books.");
        }
    }

    // GET api/<BooksController>/guid
    /// <summary>Gets a specific book.</summary>
    /// <param name="id">Id of the book.</param>
    /// <response code="200">JSON of the book.</response>
    /// <response code="404">If the book is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponse>> GetAsync(Guid id)
    {
        try
        {
            var result = await _bookService.GetBookAsync(id);
            if (!result.CheckSuccess(out var book))
            {
                return NotFound(result.Error);
            }

            var response = book.ToBookResponse();

            return Ok(response);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, $"Error getting Book with Id={id}.");
        }
    }

    // PUT api/<BooksController>/guid
    /// <summary>Updates a specific book.</summary>
    /// <param name="id">Id of the book to update.</param>
    /// <param name="request">Book with all details.</param>
    /// <response code="204">If the book has been updated.</response>
    /// <response code="404">If the book is not found.</response>
    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] BookRequest request)
    {
        try
        {
            var result = await _bookService.UpdateBookAsync(id, request);
            if (!result.CheckSuccess())
            {
                return NotFound(result.Error);
            }
            
            return NoContent();
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, $"Error updating Book with Id={id}.");
        }
    }

    // DELETE api/<BooksController>/guid
    /// <summary>Deletes a specific book.</summary>
    /// <param name="id">Id of the book to delete.</param>
    /// <response code="204">If the book has been deleted.</response>
    /// <response code="404">If the book is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        try
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result.CheckSuccess())
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, $"Error deleting Book with Id={id}.");
        }
    }
}