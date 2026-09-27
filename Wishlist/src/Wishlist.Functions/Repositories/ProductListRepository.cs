using Microsoft.EntityFrameworkCore;
using Wishlist.Core.Entities;
using Wishlist.Core.Interfaces;
using Wishlist.Functions.Data;

namespace Wishlist.Functions.Repositories;

public class ProductListRepository(WishlistDbContext dbContext) : IProductListRepository
{
    private readonly WishlistDbContext _dbContext = dbContext;
    public async Task AddAsync(ProductList list, CancellationToken cancellationToken)
    {
        await _dbContext.ProductList.AddAsync(list, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<ProductList>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        List<ProductList>? results = await _dbContext.ProductList
            .Where(l => l.CustomerId == customerId)
            .OrderBy(l => l.CreationDate)
            .ToListAsync(cancellationToken);
        return results;
    }

    public async Task<ProductList?> GetByIdAsync(Guid listId, CancellationToken cancellationToken)
    {
       ProductList? list = await _dbContext.ProductList.FindAsync(listId);
       return list;
    }

    public void Remove(ProductList list)
    {
        throw new NotImplementedException();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
       await _dbContext.SaveChangesAsync(cancellationToken);
    }
}