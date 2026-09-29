using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Wishlist.Core.Contracts;
using Wishlist.Core.Entities;
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
    [OpenApiOperation("GetAllListsByCustomerId", tags: ["CustomerRegistry"])]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<ProductListDto>))]
    public Task<IActionResult> GetAllListsByCustomerIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "customers/{customerId}/lists")] HttpRequest req,
        string customerId, FunctionContext context, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for customerId {CustomerId}", context.FunctionDefinition.Name, customerId);

            IReadOnlyList<ProductList>? result = await productListService.GetListsForCustomerAsync(customerId, cancellationToken);
            
            List<ProductListDto>? dtos = new List<ProductListDto>();

            foreach (ProductList list in result)
            {
                dtos.Add(ProductListMapper.ToJustListDto(list));
            }

            return new OkObjectResult(dtos);
        });
    }
    
    /// <summary>
    /// This creates the ProductList by CustomerId
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("CreateCustomerProductList")]
    [OpenApiOperation("CreateCustomerProductList", tags: ["CustomerRegistry"])]
    [OpenApiRequestBody(contentType: "application/json", typeof(CreateListRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(ProductListDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> CreateCustomerProductListAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous,"POST", Route = "customer/list")] HttpRequest req,
        FunctionContext context, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {

            CreateListRequest? createListRequest = await HttpReadValidatedJson.ReadValidatedJsonAsync<CreateListRequest>(req, cancellationToken);

            logger.LogInformation("Function {FunctionName} started for customerId {CustomerId}", context.FunctionDefinition.Name, createListRequest.CustomerId);
            
            ProductList list = await productListService.CreateListAsync(createListRequest, cancellationToken);

            return new CreatedResult($"list/{list.Id}", ProductListMapper.ToDto(list));
        });
    }
    
    /// <summary>
    /// This updates the list visibility.
    /// Possible values are true/false
    /// </summary>
    /// <param name="req"></param>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("UpdateProductListVisibility")]
    [OpenApiOperation("UpdateProductListVisibility", tags: ["ProductList"], Description = "Updates visibility (public / private) wishlist/gift-registry/etc. for a customer.")]
    [OpenApiRequestBody(contentType: "application/json", typeof(UpdateVisibilityRequest))]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, contentType: "application/json", typeof(ProductListDto))]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, contentType: "application/json", typeof(ErrorResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> UpdateProductListVisibilityAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "PATCH", Route = "list/{listId}/visibility")] HttpRequest req,
        FunctionContext context, Guid listId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for ListId {listId}", context.FunctionDefinition.Name, listId);

            UpdateVisibilityRequest updateProductListVisibility = await HttpReadValidatedJson.ReadValidatedJsonAsync<UpdateVisibilityRequest>(req, cancellationToken);

            ProductList productList = await productListService.SetListVisibilityAsync(listId, updateProductListVisibility.IsPublic, cancellationToken);

            return new OkObjectResult(ProductListMapper.ToJustListDto(productList));
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
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
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
    [OpenApiOperation("GetProductListByListId", tags: ["ProductList"])]
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

            ProductList? list = await productListService.GetListAsync(listId, cancellationToken);
        
            ProductListWithItemsDto listDto = ProductListMapper.ToDto(list);
        
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
    [OpenApiOperation("AddProductToWishlist", tags: ["ProductListItem"])]
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

            ProductListItem? productListItem = await productListService.AddProductAsync(listId, createListItemRequest, cancellationToken);

            logger.LogInformation("Function {FunctionName} product has been added to list {listId} and itemId {itemId} ", context.FunctionDefinition.Name, listId, productListItem.Id); 

            return new CreatedResult($"list/{listId}", "OK");
        });
    }
    
    /// <summary>
    /// Updates the ProductListItem visibility
    /// Possible values are true / false
    /// </summary>
    /// <param name="req"></param>
    /// <param name="itemId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Function("UpdateProductListItemVisibility")]
    [OpenApiOperation("UpdateProductListItemVisibility", tags: ["ProductListItem"])]
    [OpenApiRequestBody(contentType: "application/json", typeof(UpdateItemVisibilityRequest))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ProductListItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> UpdateProductListItemVisibilityAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "PATCH", Route = "items/{itemId}/visibility")] HttpRequest req,
        FunctionContext context, Guid itemId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for ItemID {itemId}", context.FunctionDefinition.Name, itemId);

            UpdateItemVisibilityRequest updateItemVisibilityRequest = await HttpReadValidatedJson.ReadValidatedJsonAsync<UpdateItemVisibilityRequest>(req, cancellationToken);

            ProductListItem? listItem = await productListService.SetItemVisibilityAsync(itemId, updateItemVisibilityRequest.IsPublic, cancellationToken);

            return new OkObjectResult(value: ProductListMapper.ToItemDto(listItem));
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
    [OpenApiOperation("DeleteProductListItemById", tags: ["ProductListItem"])]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(ErrorResponse))]
    public Task<IActionResult> DeleteProductListItemByIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "items/{itemId}/")] HttpRequest req,
        FunctionContext context,
        Guid itemId, CancellationToken cancellationToken)
    {
        return Http.RunAsync(async () =>
        {
            logger.LogInformation("Function {FunctionName} started for itemId {itemId}", context.FunctionDefinition.Name, itemId);

            await productListService.RemoveListItemAsync(itemId, cancellationToken);

            return new NoContentResult();
        });
    }
}