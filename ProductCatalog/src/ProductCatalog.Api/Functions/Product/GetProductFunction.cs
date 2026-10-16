using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProductCatalog.Api.Mappers;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Models.Responses.Product;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.Api.Functions.Product;

public sealed class GetProductFunction(IProductService productService, ILogger<GetProductFunction> logger)
{
    [Function("GetProduct")]
    [OpenApiOperation(operationId: "GetProduct", tags: ["Products"], Summary = "Get a product by id", Description = "Returns a product, enriched with image data.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The product id.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductResponse), Description = "The product.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(ErrorResponse), Description = "Product not found.")]
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
