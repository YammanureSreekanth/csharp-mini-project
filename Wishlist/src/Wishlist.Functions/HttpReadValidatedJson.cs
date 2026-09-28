using Microsoft.AspNetCore.Http;
using Wishlist.Core.Exceptions;

namespace Wishlist.Functions;

public static class HttpReadValidatedJson
{
    public static async Task<T> ReadValidatedJsonAsync<T>(HttpRequest req, CancellationToken cancellationToken)
    {
        T? body = await req.ReadFromJsonAsync<T>(cancellationToken);

        if (body is null)
        {
            throw new ValidationException("Request body is required.");
        }

        return body;
    }
}