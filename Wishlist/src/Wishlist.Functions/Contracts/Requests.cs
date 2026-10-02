using Wishlist.Core.Enums;

namespace Wishlist.Functions.Contracts;

/// <summary>
/// Request body for creating a new product list.
/// </summary>
/// <param name="Name"></param>
/// <param name="CustomerId"></param>
/// <param name="Type"></param>
/// <param name="IsPublic"></param>
public record CreateListRequest(
    string Name,
    ProductListType Type,
    bool IsPublic = false);

/// <summary>
/// Request body for creating a new product listitem.
/// </summary>
/// <param name="ProductId"></param>
/// <param name="IsPublic"></param>
/// <param name="Quantity"></param>
public record CreateProductListItemRequest(
    string ProductId,
    bool IsPublic = false,
    short Quantity = 1);

/// <summary>
/// Request body for updating the list visibility
/// </summary>
/// <param name="IsPublic"></param>
/// <param name="Quantity"></param>
public record UpdateListVisibilityRequest(
    bool? IsPublic);

/// <summary>
/// Request body for updating the listitem visibility
/// </summary>
/// <param name="IsPublic"></param>
/// <param name="Quantity"></param>
public record UpdateItemVisibilityRequest(
    bool? IsPublic,
    short? Quantity);