using Wishlist.Core.DTOs;
using Wishlist.Core.Entities;

namespace Wishlist.Core.Mapping;

/// <summary>
/// Maps ProductList/ProductListItem entities to their DTO shapes.
/// Pure functions — no dependencies, safe to unit test in isolation.
/// </summary>
public static class ProductListMapper
{
    /// <summary>
    /// Maps a list without its items — used for list/summary views.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static ProductListDto ToJustListDto(ProductList list) => new()
    {
        Id = list.Id,
        
        Name = list.Name,
        
        Type = list.Type,
        
        CustomerId = list.CustomerId,
        
        IsPublic = list.IsPublic,
        
        ModifiedDate = list.ModifiedDate
    };

    /// <summary>
    /// Maps a list including its items used for single-list detail views.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static ProductListWithItemsDto ToDto(ProductList list) => new()
    {
        Id = list.Id,
        
        Name = list.Name,
        
        Type = list.Type,
        
        CustomerId = list.CustomerId,
        
        IsPublic = list.IsPublic,
        
        ModifiedDate = list.ModifiedDate,
        
        Items = list.Items.Select(ToItemDto).ToList()
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static ProductListItemDto ToItemDto(ProductListItem item) => new()
    {
        Id = item.Id,
        
        ProductListId = item.ProductListId,
        
        ProductId = item.ProductId,
        
        IsPublic = item.IsPublic,
        
        Quantity = item.Quantity,
        
        ModifiedDate = item.ModifiedDate
    };
}