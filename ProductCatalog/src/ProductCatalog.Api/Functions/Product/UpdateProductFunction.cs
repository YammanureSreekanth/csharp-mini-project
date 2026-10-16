using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductCatalog.Api.Mappers;
using ProductCatalog.Api.Models.Requests.Product;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.Api.Functions.Product;

public sealed class UpdateProductFunction(IProductService productService, ILogger<UpdateProductFunction> logger)
{
    [Function("UpdateProduct")]
    public Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "products/{id}")] HttpRequest req,
        string id,
        CancellationToken cancellationToken) =>
        Http.RunAsync(logger, async () =>
        {
            var request = await req.ReadFromJsonAsync<UpdateProductRequest>(cancellationToken);
            var command = ProductApiMapper.ToCommand(request!) with { Id = id };
            var product = await productService.UpdateProductAsync(command, cancellationToken);

            return new OkObjectResult(ProductApiMapper.ToResponse(product));
        });
}
