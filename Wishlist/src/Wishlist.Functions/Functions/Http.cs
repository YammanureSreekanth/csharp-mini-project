using Microsoft.AspNetCore.Mvc;
using Wishlist.Core.Exceptions;

namespace Wishlist.Functions.Functions;

public static class Http
{
    public static async Task<IActionResult> RunAsync(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (ProductListNotFoundException ex) { return new NotFoundObjectResult(new { error = ex.Message }); }
        catch (DuplicateProductException ex) { return new ConflictObjectResult(new { error = ex.Message }); }
        catch (ValidationException ex)       { return new BadRequestObjectResult(new { error = ex.Message }); }
    }
}