using System.Text.Json.Serialization;
using Wishlist.Core.Enums;

namespace Wishlist.Core.Contracts;

public record CreateListRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("customerId")] string CustomerId,
    [property: JsonPropertyName("type")] ProductListType Type,
    [property: JsonPropertyName("isPublic")] bool IsPublic = false);

public record CreateProductListItemRequest(
    [property: JsonPropertyName("productId")] string ProductId,
    [property: JsonPropertyName("isPublic")] bool IsPublic = false,
    [property: JsonPropertyName("quantity")] int Quantity = 1);

public record UpdateVisibilityRequest(
    [property: JsonPropertyName("isPublic")] bool IsPublic);