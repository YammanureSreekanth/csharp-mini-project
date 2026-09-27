using Wishlist.Core.Entities;
using Wishlist.Core.Interfaces;

namespace Wishlist.Tests;

public class FakeProductListRepository : IProductListRepository
{
    public List<ProductList> Lists { get; } = new List<ProductList>(); 
    public int SaveCount { get; private set; }
   public Task<ProductList?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Lists.FirstOrDefault(l => l.Id == id));

    public Task<IReadOnlyList<ProductList>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        IReadOnlyList<ProductList> result = Lists.Where(l => l.CustomerId == customerId).ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(ProductList list, CancellationToken cancellationToken)
    {
        Lists.Add(list);
        return Task.CompletedTask;
    }

    public void Remove(ProductList list) => Lists.Remove(list);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task AddProductAsync(ProductListItem productListItem, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetItemVisibilityAsync(ProductList productList, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetListVisibilityAsync(ProductList productList, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveItemsByListId(Guid listId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ProductListItem> GetListItemByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetItemVisibilityAsync(ProductListItem productListItem, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void RemoveListItem(ProductListItem listItem, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductListItem>> GetListItemsByListId(Guid Id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}