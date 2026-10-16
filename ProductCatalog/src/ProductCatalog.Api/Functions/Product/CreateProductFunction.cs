using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProductCatalog.Api.Mappers;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Models.Requests.Product;
using ProductCatalog.Api.Models.Responses.Product;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.Api.Functions.Product;

public sealed class CreateProductFunction(IProductService productService, ILogger<CreateProductFunction> logger)
{
    [Function("CreateProduct")]
    [OpenApiOperation(operationId: "CreateProduct", tags: ["Products"], Summary = "Create a product", Description = "Creates a new product.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateProductRequest), Required = true, Description = "The product to create.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(ProductResponse), Description = "The created product.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorResponse), Description = "Validation failure.")]
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
