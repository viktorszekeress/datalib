using Datalib.WebApi.Dtos.Requests;
using Datalib.WebApi.Dtos.Responses;
using Datalib.WebApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Datalib.WebApi.Services;

namespace Datalib.WebApi.Controllers;

[Route("api/[controller]")]
public class CheckoutsController : BaseApiController
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutsController(ICheckoutService checkoutService, ILogger<CheckoutsController> logger) : base(logger)
    {
        _checkoutService = checkoutService;
    }

    // POST api/<CheckoutsController>
    /// <summary>Creates a checkout for multiple books.</summary>
    /// <param name="request">Checkout details.</param>
    /// <response code="201">JSON of the created checkout and location header.</response>
    /// <response code="400">If book ids list contains bad data.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckoutResponse>> CreateAsync([FromBody] CreateCheckoutRequest request)
    {
        try
        {
            var result = await _checkoutService.CheckOutBooksAsync(request);
            if (!result.CheckSuccess(out var checkout))
            {
                return BadRequest(new {result.Error});
            }

            var response = checkout.ToCheckoutResponse();

            return CreatedAtAction("Get", new { id = response.Id }, response);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, "Error creating Checkout.");
        }
    }

    // GET api/<CheckoutsController>
    /// <summary>Gets all checkouts for a specific user.</summary>
    /// <response code="200">JSON array of all checkouts for the user.</response>
    /// <response code="404">If user is not found.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CheckoutResponse>>> GetCheckoutsForUserAsync([FromQuery] Guid userId)
    {
        try
        {
            var result = await _checkoutService.GetCheckoutsForUserAsync(userId);
            if (!result.CheckSuccess(out var checkouts))
            {
                return NotFound(result.Error);
            }

            var responses = checkouts.Select(c => c.ToCheckoutResponse());

            return Ok(responses);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, "Error getting Checkouts for the user.");
        }
    }

    // GET api/<CheckoutsController>/guid
    /// <summary>Gets a specific checkout.</summary>
    /// <param name="id">Id of the checkout.</param>
    /// <response code="200">JSON of the checkout.</response>
    /// <response code="404">If the checkout is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CheckoutResponse>> GetAsync(Guid id)
    {
        try
        {
            var result = await _checkoutService.GetCheckoutAsync(id);
            if (!result.CheckSuccess(out var checkout))
            {
                return NotFound(result.Error);
            }

            var response = checkout.ToCheckoutResponse();

            return Ok(response);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, $"Error getting Checkout with Id={id}.");
        }
    }

    // POST api/<CheckoutsController>/guid/return
    /// <summary>Returns books from a specific checkout.</summary>
    /// <param name="checkoutId">Id of the checkout.</param>
    /// <param name="request">List of book ids.</param>
    /// <response code="204">If specified books have been successfully returned.</response>
    /// <response code="400">If any error occurs.</response>
    [HttpPost("{checkoutId}/return")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReturnBooksAsync(Guid checkoutId, [FromBody] ReturnBooksRequest request)
    {
        try
        {
            var result = await _checkoutService.ReturnBooksAsync(checkoutId, request);
            if (!result.CheckSuccess())
            {
                return BadRequest(new {result.Error});
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, "Error returning Books.");
        }
    }
}