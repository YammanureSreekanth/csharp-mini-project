namespace Wishlist.Core.DTOs;

/// <summary>
/// A product list together with its items.
/// </summary>
public class ProductListWithItemsDto : ProductListDto
{
    public List<ProductListItemDto> Items { get; set; } = new();
}