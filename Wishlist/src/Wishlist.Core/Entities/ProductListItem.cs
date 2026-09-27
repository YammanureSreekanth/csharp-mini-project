
namespace Wishlist.Core.Entities;
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