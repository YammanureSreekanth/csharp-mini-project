using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Wishlist.Core.Contracts;
using Wishlist.Core.Entities;
using Wishlist.Core.Interfaces;

namespace Wishlist.Functions.Functions;

public class ProductListFunctions(IProductListService productListService)
{
    [Function("GetAllListsByCustomerId")]
    [OpenApiOperation("GetAllListsByCustomerId", tags: ["CustomerRegistry"])]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<ProductListDto>))]
    public Task<IActionResult> GetAllListsByCustomerIdAsync(
        [HttpTrigger("GET", Route = "customers/{customerId}/lists")] HttpRequest req,
        string customerId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            IReadOnlyList<ProductList>? result = await productListService.GetListsForCustomerAsync(customerId, cancellationToken);
            
            List<ProductListDto>? dtos = new List<ProductListDto>();
            foreach (ProductList list in result)
            {
                dtos.Add(ProductListMapper.ToJustListDto(list));
            }
            return new OkObjectResult(dtos);
        });
    
    [Function("CreateCustomerProductList")]
    [OpenApiOperation("CreateCustomerProductList", tags: ["CustomerRegistry"])]
    [OpenApiRequestBody(contentType: "application/json", typeof(CreateListRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(ProductListDto))]
    public Task<IActionResult> CreateCustomerProductListAsync(
        [HttpTrigger("POST", Route = "customer/list")] HttpRequest req,
        CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {

            CreateListRequest? createListRequest = await req.ReadFromJsonAsync<CreateListRequest>(cancellationToken);
            ProductList list = await productListService.CreateListAsync(createListRequest, cancellationToken);
            return new CreatedResult($"list/{list.Id}", ProductListMapper.ToDto(list));
        });
    
    [Function("UpdateProductListVisibility")]
    [OpenApiOperation("UpdateProductListVisibility", tags: ["ProductList"], Description = "Updates visibility (public / private) wishlist/gift-registry/etc. for a customer.")]
    [OpenApiRequestBody(contentType: "application/json", typeof(UpdateVisibilityRequest))]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, contentType: "application/json", typeof(ProductListDto))]
    public Task<IActionResult> UpdateProductListVisibilityAsync(
        [HttpTrigger("PATCH", Route = "list/{listId}/visibility")] HttpRequest req,
        Guid listId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            UpdateVisibilityRequest updateProductListVisibility = await req.ReadFromJsonAsync<UpdateVisibilityRequest>(cancellationToken);
            ProductList productList = await productListService.SetListVisibilityAsync(listId, updateProductListVisibility.IsPublic, cancellationToken);
            return new OkObjectResult(ProductListMapper.ToJustListDto(productList));
        });
    
    [Function("RemoveProductList")]
    [OpenApiOperation("RemoveProductList", tags: ["ProductList"], Description = "Deletes the current list from Customer")]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    public Task<IActionResult> RemoveProductListAsync(
        [HttpTrigger("DELETE", Route = "list/{listId}")] HttpRequest req,
        Guid listId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            await productListService.DeleteListAsync(listId, cancellationToken);
            return new NoContentResult();
        });
    
    [Function("GetProductListByListId")]
    [OpenApiOperation("GetProductListByListId", tags: ["ProductList"])]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ProductListWithItemsDto))]
    public Task<IActionResult> GetProductListByListIdAsync(
        [HttpTrigger("GET", Route = "list/{listId}")] HttpRequest req,
        Guid listId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            ProductList list = await productListService.GetListAsync(listId, cancellationToken);
            ProductListWithItemsDto listDto = ProductListMapper.ToDto(list);
            return new OkObjectResult(value: listDto);
        });
    
    [Function("AddProductToList")]
    [OpenApiOperation("AddProductToWishlist", tags: ["ProductListItem"])]
    [OpenApiRequestBody(contentType: "application/json", typeof(CreateProductListItemRequest))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(string))]
    public Task<IActionResult> AddProductToListAsync(
        [HttpTrigger("POST", Route = "list/{listId}/items")] HttpRequest req,
        Guid listId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            CreateProductListItemRequest? createListItemRequest = await req.ReadFromJsonAsync<CreateProductListItemRequest>(cancellationToken);
            await productListService.AddProductAsync(listId, createListItemRequest, cancellationToken);
            return new CreatedResult($"list/{listId}", "OK");
        });
    
    [Function("UpdateProductListItemVisibility")]
    [OpenApiOperation("UpdateProductListItemVisibility", tags: ["ProductListItem"])]
    [OpenApiRequestBody(contentType: "application/json", typeof(UpdateVisibilityRequest))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ProductListItemDto))]
    public Task<IActionResult> UpdateProductListItemVisibilityAsync(
        [HttpTrigger("PATCH", Route = "items/{itemId}/visibility")] HttpRequest req,
        Guid itemId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            UpdateVisibilityRequest updateProductListVisibility = await req.ReadFromJsonAsync<UpdateVisibilityRequest>(cancellationToken);
            ProductListItem listItem = await productListService.SetItemVisibilityAsync(itemId, updateProductListVisibility.IsPublic, cancellationToken);
            return new OkObjectResult(value: ProductListMapper.ToItemDto(listItem));
        });
    
    [Function("DeleteProductListItemById")]
    [OpenApiOperation("DeleteProductListItemById", tags: ["ProductListItem"])]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    public Task<IActionResult> DeleteProductListItemByIdAsync(
        [HttpTrigger("DELETE", Route = "items/{itemId}/")] HttpRequest req,
        Guid itemId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            UpdateVisibilityRequest updateProductListVisibility = await req.ReadFromJsonAsync<UpdateVisibilityRequest>(cancellationToken);
            ProductListItem listItem = await productListService.SetItemVisibilityAsync(itemId, updateProductListVisibility.IsPublic, cancellationToken);
            return new OkObjectResult(value: ProductListMapper.ToItemDto(listItem));
        });
}