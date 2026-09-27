using Wishlist.Core.Entities;
using Wishlist.Core.Enums;

namespace Wishlist.Core.Contracts;

public class ProductListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public ProductListType Type {get; set;}
    public bool IsPublic { get; set; }
    public DateTime ModifiedDate { get; set; }
}