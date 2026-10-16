using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductCatalog.Api.Mappers;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.Api.Functions.Product;

public sealed class GetProductFunction(IProductService productService, ILogger<GetProductFunction> logger)
{
    [Function("GetProduct")]
    public Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] HttpRequest req,
        string id,
        CancellationToken cancellationToken) =>
        Http.RunAsync(logger, async () =>
        {
            var details = await productService.GetProductAsync(id, cancellationToken);

            return new OkObjectResult(ProductApiMapper.ToResponse(details));
        });
}
