using Wishlist.Core.Enums;

namespace Wishlist.Core.Entities;
public class ProductList
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ProductListType Type {get; set;}
    public required string CustomerId {get; set;}
    public bool IsPublic { get; set; } = true;
    public DateTime CreationDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public List<ProductListItem> Items { get; set; } = [];
}
