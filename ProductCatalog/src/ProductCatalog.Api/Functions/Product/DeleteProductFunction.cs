using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProductCatalog.Api.Models;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.Api.Functions.Product;

public sealed class DeleteProductFunction(IProductService productService, ILogger<DeleteProductFunction> logger)
{
    [Function("DeleteProduct")]
    [OpenApiOperation(operationId: "DeleteProduct", tags: ["Products"], Summary = "Delete a product", Description = "Deletes a product by id.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The product id.")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "The product was deleted.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(ErrorResponse), Description = "Product not found.")]
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
