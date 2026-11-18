using LootHub.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Failure(ErrorType? errorType, string message)
    {
        var statusCode = errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.PaymentRequired => StatusCodes.Status402PaymentRequired,
            _ => StatusCodes.Status500InternalServerError,

        };

        return Problem(statusCode: statusCode, detail: message);
    }

    protected Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(sub, out var userId))
                throw new InvalidOperationException("The 'sub' claim is missing or invalid. Is [Authorize] missing on this endpoint?");

            return userId;
        }
    }
}
