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
    [Function("GetCustomerLists")]
    [OpenApiOperation("GetCustomerLists", tags: ["ProductLists"])]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<ProductListDto>))]
    public Task<IActionResult> GetCustomerListAsync(
        [HttpTrigger("get", Route = "customers/{customerId}/lists")] HttpRequest req,
        string customerId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            IReadOnlyList<ProductList>? result = await productListService.GetListsForCustomerAsync(customerId, cancellationToken);
            
            List<ProductListDto>? dtos = new List<ProductListDto>();
            foreach (ProductList list in result)
            {
                dtos.Add(ProductListMapper.ToDto(list));
            }
            return new OkObjectResult(dtos);
        });
    
    [Function("CreateCustomerList")]
    [OpenApiOperation("CreateCustomerList", tags: ["ProductList"])]
    [OpenApiRequestBody(contentType: "application/json", typeof(CreateListRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(ProductListDto))]
    public Task<IActionResult> CreateCustomerListAsync(
        [HttpTrigger("post", Route = "customer/list")] HttpRequest req,
        CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {

            CreateListRequest? createListRequest = await req.ReadFromJsonAsync<CreateListRequest>(cancellationToken);
            ProductList list = await productListService.CreateListAsync(createListRequest, cancellationToken);
            return new CreatedResult($"customer/{list.CustomerId}/list/{list.Id}", ProductListMapper.ToDto(list));
        });
    
    [Function("GetCustomerListByListId")]
    [OpenApiOperation("GetCustomerListByListId", tags: ["ProductList"])]
    [OpenApiParameter("customerId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ProductListDto))]
    public Task<IActionResult> GetCustomerListByListIdAsync(
        [HttpTrigger("get", Route = "customer/{customerId}/list/{listId}")] HttpRequest req,
        string customerId, Guid listId, CancellationToken cancellationToken) => Http.RunAsync(async () =>
        {
            ProductList list = await productListService.GetListAsync(listId, cancellationToken);
            ProductListDto listDto = ProductListMapper.ToDto(list);
            return new OkObjectResult(value: listDto);
        });
}