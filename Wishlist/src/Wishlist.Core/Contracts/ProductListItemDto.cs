namespace Wishlist.Core.Contracts;

/// <summary>
/// A product list item as returned by the API
/// Exposes only what are needed and internal fields such as CreationDate are left out.
/// </summary>
public class ProductListItemDto
{
    public required Guid Id { get; set; }
    public required Guid ProductListId { get; set; }
    public required string ProductId {get; set;}
    public bool IsPublic { get; set; } = true;
    public short Quantity {get; set;} = 1;
    public DateTime ModifiedDate { get; set; }
}