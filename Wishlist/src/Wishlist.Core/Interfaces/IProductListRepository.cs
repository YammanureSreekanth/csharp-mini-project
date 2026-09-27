using Wishlist.Core.Entities;

namespace Wishlist.Core.Interfaces;

public interface IProductListRepository
{
    Task<ProductList?> GetByIdAsync(Guid listId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductList>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task AddAsync(ProductList list, CancellationToken cancellationToken);
    void Remove(ProductList list);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
 