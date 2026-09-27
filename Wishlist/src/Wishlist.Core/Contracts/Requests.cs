using System.Text.Json.Serialization;
using Wishlist.Core.Enums;

namespace Wishlist.Core.Contracts;

public record CreateListRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("customerId")] string CustomerId,
    [property: JsonPropertyName("type")] ProductListType Type,
    [property: JsonPropertyName("isPublic")] bool IsPublic = false);

public record AddProductRequest(string ProductId);
public record UpdateVisibilityRequest(bool IsPublic);