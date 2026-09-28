using Wishlist.Core.Contracts;
using Wishlist.Core.Entities;

namespace Wishlist.Core.Interfaces;

/// <summary>
/// This is Interface
/// </summary>
public interface IProductListService
{
    Task<ProductList> CreateListAsync(CreateListRequest req, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductList>> GetListsForCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<ProductList> GetListAsync(Guid listId, CancellationToken cancellationToken);
    Task DeleteListAsync(Guid listId, CancellationToken cancellationToken);
    Task<ProductList> SetListVisibilityAsync(Guid productListId, bool isPublic, CancellationToken cancellationToken);
    Task AddProductAsync(Guid Id, CreateProductListItemRequest req, CancellationToken cancellationToken);
    Task RemoveListItemAsync(Guid listId, CancellationToken cancellationToken);
    Task<ProductListItem> SetItemVisibilityAsync(Guid itemId, bool isPublic, CancellationToken cancellationToken);
}