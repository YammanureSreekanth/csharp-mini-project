using System.Text.Json.Serialization;
using Wishlist.Core.Enums;

namespace Wishlist.Core.Contracts;

/// <summary>
/// Request body for creating a new product list.
/// </summary>
/// <param name="Name"></param>
/// <param name="CustomerId"></param>
/// <param name="Type"></param>
/// <param name="IsPublic"></param>
public record CreateListRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("customerId")] string CustomerId,
    [property: JsonPropertyName("type")] ProductListType Type,
    [property: JsonPropertyName("isPublic")] bool IsPublic = false);

/// <summary>
/// Request body for creating a new product listitem.
/// </summary>
/// <param name="ProductId"></param>
/// <param name="IsPublic"></param>
/// <param name="Quantity"></param>
public record CreateProductListItemRequest(
    [property: JsonPropertyName("productId")] string ProductId,
    [property: JsonPropertyName("isPublic")] bool IsPublic = false,
    [property: JsonPropertyName("quantity")] int Quantity = 1);

/// <summary>
/// Request body for updating the visibility
/// </summary>
/// <param name="IsPublic"></param>
public record UpdateVisibilityRequest(
    [property: JsonPropertyName("isPublic")] bool IsPublic);