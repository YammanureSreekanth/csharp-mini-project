using Wishlist.Core.Contracts;
using Wishlist.Core.Entities;

namespace Wishlist.Core.Interfaces;

public interface IProductListService
{
    Task<ProductList> CreateListAsync(CreateListRequest req, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductList>> GetListsForCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<ProductList> GetListAsync(Guid listId, CancellationToken cancellationToken);
    Task DeleteListAsync(Guid listId);
    Task SetListVisibilityAsync(Guid listId, bool isPublic);
    Task AddProductAsync(Guid listId, string productId);
    Task RemoveProductAsync(Guid listId, string productId);
    Task SetItemVisibilityAsync(Guid listId, string productId, bool isPublic);
    Task<ProductList> GetPublicListAsync(Guid listId);
}