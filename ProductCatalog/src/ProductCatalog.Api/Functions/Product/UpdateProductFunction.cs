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

public sealed class UpdateProductFunction(IProductService productService, ILogger<UpdateProductFunction> logger)
{
    [Function("UpdateProduct")]
    [OpenApiOperation(operationId: "UpdateProduct", tags: ["Products"], Summary = "Update a product", Description = "Updates an existing product's details.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The product id.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateProductRequest), Required = true, Description = "The updated product details.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductResponse), Description = "The updated product.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(ErrorResponse), Description = "Product not found.")]
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
