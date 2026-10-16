using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductCatalog.Api.Mappers;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.Api.Functions.Product;

public sealed class GetAllProductsFunction(IProductService productService, ILogger<GetAllProductsFunction> logger)
{
    [Function("GetAllProducts")]
    public Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequest req,
        CancellationToken cancellationToken) =>
        Http.RunAsync(logger, async () =>
        {
            var products = await productService.GetAllProductsAsync(cancellationToken);

            return new OkObjectResult(products.Select(ProductApiMapper.ToResponse));
        });
}
