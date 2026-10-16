using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductCatalog.Api.Mappers;
using ProductCatalog.Api.Models.Requests.Product;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.Api.Functions.Product;

public sealed class CreateProductFunction(IProductService productService, ILogger<CreateProductFunction> logger)
{
    [Function("CreateProduct")]
    public Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequest req,
        CancellationToken cancellationToken) =>
        Http.RunAsync(logger, async () =>
        {
            var request = await req.ReadFromJsonAsync<CreateProductRequest>(cancellationToken);
            var command = ProductApiMapper.ToCommand(request!);
            var product = await productService.CreateProductAsync(command, cancellationToken);

            return new CreatedResult($"/products/{product.Id}", ProductApiMapper.ToResponse(product));
        });
}
