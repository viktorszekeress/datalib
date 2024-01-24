using System.Net.Mime;
using Datalib.WebApi.Dtos;
using Datalib.WebApi.Dtos.Requests;
using Datalib.WebApi.Dtos.Responses;
using Datalib.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datalib.WebApi.Controllers;

[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService, ILogger<UsersController> logger) : base(logger)
    {
        _userService = userService;
    }

    // POST api/<UsersController>
    /// <summary>Creates a new user.</summary>
    /// <param name="request">User to create.</param>
    /// <response code="201">JSON of the created user and location header.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponse>> CreateAsync([FromBody] UserRequest request)
    {
        try
        {
            var result = await _userService.CreateUserAsync(request);
            if (!result.CheckSuccess(out var user))
            {
                return BadRequest(new {result.Error});
            }

            var response = user.ToUserResponse();

            return CreatedAtAction("Get", new { id = response.Id }, response);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, "Error creating User.");
        }
    }

    // GET api/<UsersController>
    /// <summary>Gets all users.</summary>
    /// <response code="200">JSON array of all users.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAsync()
    {
        try
        {
            var result = await _userService.GetAllUsersAsync();
            if (!result.CheckSuccess(out var users))
            {
                return BadRequest(new {result.Error});
            }

            var responses = users.Select(b => b.ToUserResponse());

            return Ok(responses);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, "Error getting Users.");
        }
    }

    // GET api/<UsersController>/guid
    /// <summary>Gets a specific user.</summary>
    /// <param name="id">Id of the user.</param>
    /// <response code="200">JSON of the user.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetAsync(Guid id)
    {
        try
        {
            var result = await _userService.GetUserAsync(id);
            if (!result.CheckSuccess(out var user))
            {
                return NotFound(result.Error);
            }

            var response = user.ToUserResponse();

            return Ok(response);
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, $"Error getting User with Id={id}.");
        }
    }

    // PUT api/<UsersController>/guid
    /// <summary>Updates a specific user.</summary>
    /// <param name="id">Id of the user to update.</param>
    /// <param name="request">User with all details.</param>
    /// <response code="204">If the user has been updated.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UserRequest request)
    {
        try
        {
            var result = await _userService.UpdateUserAsync(id, request);
            if (!result.CheckSuccess())
            {
                return NotFound(result.Error);
            }
            
            return NoContent();
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, $"Error updating User with Id={id}.");
        }
    }

    // DELETE api/<UsersController>/guid
    /// <summary>Deletes a specific user.</summary>
    /// <param name="id">Id of the user to delete.</param>
    /// <response code="204">If the user has been deleted.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.CheckSuccess())
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return await HandleErrorAsync(e, $"Error deleting User with Id={id}.");
        }
    }
}