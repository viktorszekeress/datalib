using Microsoft.AspNetCore.Mvc;

namespace Datalib.WebApi.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    private readonly ILogger _logger;

    protected BaseApiController(ILogger logger)
    {
        _logger = logger;
    }

    protected Task<ObjectResult> HandleErrorAsync(Exception e, string message)
    {
        _logger.LogError(e, message);
        
        return Task.FromResult(Problem(message));
    }
}