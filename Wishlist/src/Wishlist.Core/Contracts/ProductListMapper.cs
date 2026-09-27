using Wishlist.Core.Entities;

namespace Wishlist.Core.Contracts;

public static class ProductListMapper
{
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

    public static ProductListItemDto ToItemDto(ProductListItem listItem)
    {
        ProductListItemDto? itemDto = new ProductListItemDto
        {
            Id = listItem.Id,
            ProductListId = listItem.ProductListId,
            ProductId = listItem.ProductId,
            IsPublic = listItem.IsPublic,
            Quantity = listItem.Quantity
        };

        return itemDto;
    }
}