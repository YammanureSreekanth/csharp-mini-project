using Wishlist.Core.Enums;

namespace Wishlist.Core.Contracts;

public class ProductListWithItemsDto : ProductListDto
{
    public List<ProductListItemDto> Items { get; set; } = new();
}