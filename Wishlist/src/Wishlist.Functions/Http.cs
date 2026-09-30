using Microsoft.AspNetCore.Mvc;
using Wishlist.Core.Exceptions;
using Wishlist.Core.Contracts;

namespace Wishlist.Functions;

/// <summary>
/// This Returns delegates for Functions
/// </summary>
public static class Http
{
    /// <summary>
    /// This has Fun delete to peforms the results.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<IActionResult> RunAsync(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (NotFoundException ex)            { return new NotFoundObjectResult(new ErrorResponse(ex.Message)); }
        catch (ConflictProductException ex)     { return new ConflictObjectResult(new ErrorResponse(ex.Message)); }
        catch (ValidationException ex)          { return new BadRequestObjectResult(new ErrorResponse(ex.Message)); }
    }
}