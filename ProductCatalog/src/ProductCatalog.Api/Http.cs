using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductCatalog.Api.Models;
using ProductCatalog.App.Models.Exceptions;

namespace ProductCatalog.Api;

/// <summary>
/// This Returns delegates for Functions
/// </summary>
public static class Http
{
    /// <summary>
    /// This has Fun delete to peforms the results.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<IActionResult> RunAsync(ILogger logger, Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (ProductNotFoundException ex)   { return new NotFoundObjectResult(new ErrorResponse(ex.Message)); }
        catch (ProductValidationException ex) { return new BadRequestObjectResult(new ErrorResponse(ex.Message)); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing request.");

            return new ObjectResult(new ErrorResponse("An unexpected error occurred."))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
