using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.Api.Functions.Product;

public sealed class DeleteProductFunction(IProductService productService, ILogger<DeleteProductFunction> logger)
{
    [Function("DeleteProduct")]
    public Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] HttpRequest req,
        string id,
        CancellationToken cancellationToken) =>
        Http.RunAsync(logger, async () =>
        {
            await productService.DeleteProductAsync(id, cancellationToken);

            return new NoContentResult();
        });
}
