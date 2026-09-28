using Wishlist.Core.Entities;

namespace Wishlist.Core.Contracts;

/// <summary>
/// Converts <see cref="ProductList"/> entities to DTOs for API responses.
/// </summary>
public static class ProductListMapper
{
    /// <summary>
    /// Maps a list to its DTO, wit items
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static ProductListWithItemsDto ToDto(ProductList list)
    {
        ProductListWithItemsDto? dto = new ProductListWithItemsDto();
        
        dto.Id = list.Id;
        
        dto.Name = list.Name;
        
        dto.Type = list.Type;
        
        dto.ModifiedDate = list.ModifiedDate;
        
        dto.IsPublic = list.IsPublic;

        foreach (ProductListItem item in list.Items)
        {
            ProductListItemDto? itemDto = new ProductListItemDto
            {
                Id = item.Id,
                
                ProductListId = item.ProductListId,
                
                ProductId = item.ProductId,
                
                IsPublic = item.IsPublic,
                
                Quantity = item.Quantity,
            };
            
            itemDto.ModifiedDate = item.ModifiedDate;
            
            dto.Items.Add(itemDto);
        }

        return dto;
    }

    /// <summary>
    /// Maps a list to its DTO, without items
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static ProductListDto ToJustListDto(ProductList list)
    {
        ProductListDto? dto = new ProductListDto();
        
        dto.Id = list.Id;
        
        dto.Name = list.Name;
        
        dto.Type = list.Type;
        
        dto.ModifiedDate = list.ModifiedDate;
        
        dto.IsPublic = list.IsPublic;
        
        return dto;
    }

    /// <summary>
    /// Maps a listItem to its DTO
    /// </summary>
    /// <param name="listItem"></param>
    /// <returns></returns>
    public static ProductListItemDto ToItemDto(ProductListItem listItem)
    {
        ProductListItemDto? itemDto = new ProductListItemDto
        {
            Id = listItem.Id,
            
            ProductListId = listItem.ProductListId,
            
            ProductId = listItem.ProductId,
            
            IsPublic = listItem.IsPublic,
            
            Quantity = listItem.Quantity,

            ModifiedDate = listItem.ModifiedDate
        };

        return itemDto;
    }
}