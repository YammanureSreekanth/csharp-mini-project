namespace Wishlist.Core.Entities;

/// <summary>
/// This is ProductListItem Class which can have product assigned along with visibility
/// </summary>
public class ProductListItem
{
    public Guid Id { get; set; }
    public Guid ProductListId { get; set; }
    public required ProductList List {get; set;}
    public required string ProductId {get; set;}
    public bool IsPublic { get; set; } = true;
    public short Quantity {get; set;} = 1;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; } 
}