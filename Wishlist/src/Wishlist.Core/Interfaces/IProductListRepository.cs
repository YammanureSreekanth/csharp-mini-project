using Wishlist.Core.Entities;

namespace Wishlist.Core.Interfaces;

/// <summary>
/// This is ProductList Repository to perform all DB CURD operations
/// This performs both ProductList & ProductListItem
/// </summary>
public interface IProductListRepository
{
    Task<ProductList?> GetByIdAsync(Guid listId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductList>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task AddAsync(ProductList list, CancellationToken cancellationToken);
    Task UpdateListAsync(ProductList productList, CancellationToken cancellationToken);
    void RemoveList(ProductList list);
    Task AddProductAsync(ProductListItem productListItem, CancellationToken cancellationToken);
    Task<List<ProductListItem>> GetListItemsByListId(Guid Id, CancellationToken cancellationToken);
    Task<ProductListItem?> GetListItemByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<bool> ExistProductIdByListId(Guid listId, string productId, CancellationToken cancellationToken);
    Task UpdateListItemAsync(ProductListItem productListItem, CancellationToken cancellationToken);
    void RemoveListItem(ProductListItem listItem, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
 