namespace Middlewares;

class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"Path is {context.Request.Path}");
        Console.WriteLine($"Is this Secure protocal {context.Request.IsHttps}");
        Console.WriteLine($"Status code of Request: {context.Response.StatusCode}");
        Console.WriteLine("------------------------------------------------------");
        // context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await _next(context);
    }
}