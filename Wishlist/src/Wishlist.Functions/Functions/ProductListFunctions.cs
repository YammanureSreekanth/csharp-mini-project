using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Wishlist.Core.Contracts;
using Wishlist.Core.DTOs;
using Wishlist.Core.Interfaces;

namespace Wishlist.Functions;

/// <summary>
/// The ProductList Function App
/// </summary>
/// <param name="productListService"></param>
public class ProductListFunctions(IProductListService productListService, ILogger<ProductListFunctions> logger)
{   
    /// <summary>
    /// This Gets all lists by CustomerId
    /// </summary>
    /// <param name="req"></param>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("GetAllListsByCustomerId")]
    [OpenApiOperation("GetAllListsByCustomerId", tags: ["CustomerRegistry"], Description = "This Gets all lists by CustomerId")]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<ProductListDto>))]
    public Task<IActionResult> GetAllListsByCustomerIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "customers/{customerId}/lists")] HttpRequest req,
        string customerId, FunctionContext context, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for customerId {CustomerId}", context.FunctionDefinition.Name, customerId);

            IReadOnlyList<ProductListDto>? result = await productListService.GetListsForCustomerAsync(customerId, cancellationToken);
            
            return new OkObjectResult(result);
        });
    }
    
    /// <summary>
    /// This creates the ProductList by CustomerId
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("CreateCustomerProductList")]
    [OpenApiOperation("CreateCustomerProductList", tags: ["CustomerRegistry"], Description = "This creates the ProductList by CustomerId")]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiRequestBody(contentType: "application/json", typeof(CreateListRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(ProductListDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> CreateCustomerProductListAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous,"POST", Route = "customer/{customerId}/list")] HttpRequest req,
        string customerId,
        FunctionContext context, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {

            CreateListRequest? createListRequest = await HttpReadValidatedJson.ReadValidatedJsonAsync<CreateListRequest>(req, cancellationToken);

            logger.LogInformation("Function {FunctionName} started for customerId {CustomerId}", context.FunctionDefinition.Name, customerId);
            
            ProductListDto dtoList = await productListService.CreateListAsync(customerId, createListRequest, cancellationToken);

            return new CreatedResult($"list/{dtoList.Id}", dtoList);
        });
    }
    
    /// <summary>
    /// This updates the list.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("UpdateProductList")]
    [OpenApiOperation("UpdateProductListVisibility", tags: ["ProductList"], Description = "Updates visibility (public / private) wishlist/gift-registry/etc. for a customer.")]
    [OpenApiRequestBody(contentType: "application/json", typeof(UpdateListVisibilityRequest))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, contentType: "application/json", typeof(ProductListDto))]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, contentType: "application/json", typeof(ErrorResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> UpdateProductListVisibilityAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "PATCH", Route = "list/{listId}")] HttpRequest req,
        FunctionContext context, Guid listId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for ListId {listId}", context.FunctionDefinition.Name, listId);

            UpdateListVisibilityRequest updateProductListVisibility = await HttpReadValidatedJson.ReadValidatedJsonAsync<UpdateListVisibilityRequest>(req, cancellationToken);

            ProductListDto productListDto = await productListService.UpdateListAsync(listId, updateProductListVisibility, cancellationToken);

            return new OkObjectResult(productListDto);
        });
    }
    
    /// <summary>
    /// Removes the productlist along with items assigned to list from customer
    /// </summary>
    /// <param name="req"></param>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("RemoveProductList")]
    [OpenApiOperation("RemoveProductList", tags: ["ProductList"], Description = "Deletes the current list from Customer")]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, contentType: "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> RemoveProductListAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "list/{listId}")] HttpRequest req,
        FunctionContext context, Guid listId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for ListId {listId}", context.FunctionDefinition.Name, listId);

            await productListService.DeleteListAsync(listId, cancellationToken);

            return new NoContentResult();
        });
    }
    
    /// <summary>
    /// Gets the ProductList along with listItems
    /// </summary>
    /// <param name="req"></param>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("GetProductListByListId")]
    [OpenApiOperation("GetProductListByListId", tags: ["ProductList"], Description = "Gets the ProductList along with listItems")]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ProductListWithItemsDto))]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, contentType: "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> GetProductListByListIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "list/{listId}")] HttpRequest req,
        FunctionContext context, Guid listId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for ListId {listId}", context.FunctionDefinition.Name, listId);

            ProductListWithItemsDto listDto = await productListService.GetListAsync(listId, cancellationToken);
                
            return new OkObjectResult(value: listDto);
        });
    }
    
    /// <summary>
    /// Adds the product to ProductList
    /// </summary>
    /// <param name="req"></param>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("AddProductToList")]
    [OpenApiOperation("AddProductToWishlist", tags: ["ProductListItem"], Description = "Adds the product to ProductList")]
    [OpenApiRequestBody(contentType: "application/json", typeof(CreateProductListItemRequest))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(ErrorResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.Conflict, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> AddProductToListAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "list/{listId}/items")] HttpRequest req,
        FunctionContext context, Guid listId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for ListId {listId}", context.FunctionDefinition.Name, listId);

            CreateProductListItemRequest? createListItemRequest = await HttpReadValidatedJson.ReadValidatedJsonAsync<CreateProductListItemRequest>(req, cancellationToken);

            ProductListItemDto? productListItem = await productListService.AddProductAsync(listId, createListItemRequest, cancellationToken);

            logger.LogInformation("Function {FunctionName} product has been added to list {listId} and itemId {itemId} ", context.FunctionDefinition.Name, listId, productListItem.Id); 

            return new CreatedResult($"list/{listId}", "OK");
        });
    }
    
    /// <summary>
    /// Updates the ProductListItem
    /// </summary>
    /// <param name="req"></param>
    /// <param name="itemId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("UpdateProductListItem")]
    [OpenApiOperation("UpdateProductListItemVisibility", tags: ["ProductListItem"], Description = "Updates the ProductListItem visibility")]
    [OpenApiRequestBody(contentType: "application/json", typeof(UpdateItemVisibilityRequest))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ProductListItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> UpdateProductListItemVisibilityAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "PATCH", Route = "lists/{listId}/items/{itemId}")] HttpRequest req,
        FunctionContext context, Guid listId, Guid itemId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for ItemID {itemId}", context.FunctionDefinition.Name, itemId);

            UpdateItemVisibilityRequest updateItemVisibilityRequest = await HttpReadValidatedJson.ReadValidatedJsonAsync<UpdateItemVisibilityRequest>(req, cancellationToken);

            ProductListItemDto listItemDto = await productListService.UpdateListItemAsync(listId, itemId, updateItemVisibilityRequest, cancellationToken);

            return new OkObjectResult(value: listItemDto);
        });
    }
    
    /// <summary>
    /// Deletes the ProductListItem from List
    /// </summary>
    /// <param name="req"></param>
    /// <param name="itemId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("DeleteProductListItemById")]
    [OpenApiOperation("DeleteProductListItemById", tags: ["ProductListItem"], Description = "Deletes the ProductListItem from List")]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> DeleteProductListItemByIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "lists/{listId}/items/{itemId}")] HttpRequest req,
        FunctionContext context,
        Guid listId, Guid itemId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for itemId {itemId}", context.FunctionDefinition.Name, itemId);

            await productListService.RemoveListItemAsync(listId, itemId, cancellationToken);

            return new NoContentResult();
        });
    }
}