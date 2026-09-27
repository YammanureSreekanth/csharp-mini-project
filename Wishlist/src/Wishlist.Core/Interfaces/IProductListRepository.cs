using Wishlist.Core.Entities;

namespace Wishlist.Core.Interfaces;

public interface IProductListRepository
{
    Task<ProductList?> GetByIdAsync(Guid listId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductList>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task AddAsync(ProductList list, CancellationToken cancellationToken);
    Task SetListVisibilityAsync(ProductList productList, CancellationToken cancellationToken);
    Task RemoveItemsByListId(Guid listId, CancellationToken cancellationToken);
    void Remove(ProductList list);
    Task AddProductAsync(ProductListItem productListItem, CancellationToken cancellationToken);
    Task<List<ProductListItem>> GetListItemsByListId(Guid Id, CancellationToken cancellationToken);
    Task<ProductListItem> GetListItemByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task SetItemVisibilityAsync(ProductListItem productListItem, CancellationToken cancellationToken);
    void RemoveListItem(ProductListItem listItem, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
 