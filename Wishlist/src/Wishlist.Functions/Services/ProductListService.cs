using Wishlist.Core.Contracts;
using Wishlist.Core.Entities;
using Wishlist.Core.Interfaces;

namespace Wishlist.Functions.Services;

public class ProductListService(IProductListRepository repo) : IProductListService
{
    public async Task<ProductList> CreateListAsync(CreateListRequest req, CancellationToken cancellationToken)
    {
        ProductList list = new ProductList { Id = Guid.NewGuid(), Name = req.Name, CustomerId = req.CustomerId };
        list.IsPublic = req.IsPublic;
        list.Type = req.Type;
        list.CreationDate = DateTime.UtcNow;
        list.ModifiedDate = DateTime.UtcNow;

        await repo.AddAsync(list, cancellationToken);
        await repo.SaveChangesAsync(cancellationToken);

        return list;
    }

    public async Task<IReadOnlyList<ProductList>> GetListsForCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
       return await repo.GetByCustomerAsync(customerId, cancellationToken);
    }

    public async Task<ProductList?> GetListAsync(Guid listId, CancellationToken cancellationToken)
    {
       ProductList? list = await repo.GetByIdAsync(listId, cancellationToken);
       return list;
    }

    public Task DeleteListAsync(Guid listId)
    {
        throw new NotImplementedException();
    }

    public Task SetListVisibilityAsync(Guid listId, bool isPublic)
    {
        throw new NotImplementedException();
    }

    public Task AddProductAsync(Guid listId, string productId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveProductAsync(Guid listId, string productId)
    {
        throw new NotImplementedException();
    }

    public Task SetItemVisibilityAsync(Guid listId, string productId, bool isPublic)
    {
        throw new NotImplementedException();
    }

    public Task<ProductList> GetPublicListAsync(Guid listId)
    {
        throw new NotImplementedException();
    }
}