using Wishlist.Core.Enums;

namespace Wishlist.Core.Contracts;

/// <summary>
/// A product list as returned by the API, without its items.
/// Exposes only what are needed and internal fields such as CustomerId are left out.
/// </summary>
public class ProductListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public ProductListType Type {get; set;}
    public bool IsPublic { get; set; }
    public DateTime ModifiedDate { get; set; }
}