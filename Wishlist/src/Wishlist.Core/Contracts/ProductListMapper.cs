using Wishlist.Core.Entities;

namespace Wishlist.Core.Contracts;

public static class ProductListMapper
{
    public static ProductListDto ToDto(ProductList list)
    {
        ProductListDto? dto = new ProductListDto();
        dto.Id = list.Id;
        dto.Name = list.Name;
        dto.Type = list.Type;
        dto.ModifiedDate = list.ModifiedDate;
        dto.IsPublic = list.IsPublic;

        foreach (var item in list.Items)
        {
            ProductListItemDto? itemDto = new ProductListItemDto
            {
                Id = item.Id,
                ProductListId = item.ProductListId,
                ProductId = item.ProductId,
                IsPublic = item.IsPublic,
                Quantity = item.Quantity,
                List = item.List
            };
            itemDto.ModifiedDate = item.ModifiedDate;
            dto.Items.Add(itemDto);
        }

        return dto;
    }
}