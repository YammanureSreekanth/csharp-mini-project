using Wishlist.Core.Entities;

namespace Wishlist.Core.Contracts;

public class ProductListItemDto
{
    public required Guid Id { get; set; }
    public required Guid ProductListId { get; set; }
    public required string ProductId {get; set;}
    public bool IsPublic { get; set; } = true;
    public short Quantity {get; set;} = 1;
    public DateTime ModifiedDate { get; set; }
}