using Microsoft.EntityFrameworkCore;
using Wishlist.Core.Entities;
using Wishlist.Core.Interfaces;
using Wishlist.Functions.Data;

namespace Wishlist.Functions.Repositories;

public class ProductListRepository(WishlistDbContext dbContext) : IProductListRepository
{
    private readonly WishlistDbContext _dbContext = dbContext;
    public async Task<IReadOnlyList<ProductList>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        List<ProductList>? results = await _dbContext.ProductList
            .Where(l => l.CustomerId == customerId)
            .OrderBy(l => l.CreationDate)
            .ToListAsync(cancellationToken);
        return results;
    }

    public async Task AddAsync(ProductList list, CancellationToken cancellationToken)
    {
        await _dbContext.ProductList.AddAsync(list, cancellationToken: cancellationToken);
    }

    public async Task<ProductList?> GetByIdAsync(Guid listId, CancellationToken cancellationToken)
    {
       ProductList? list = await _dbContext.ProductList.FindAsync(listId);
       return list;
    }

    public async Task SetListVisibilityAsync(ProductList productList, CancellationToken cancellationToken)
    {
        _dbContext.ProductList.Update(productList);
    }

    public void Remove(ProductList list)
    {
       _dbContext.ProductList.Remove(list);
    }

    public async Task RemoveItemsByListId(Guid listId, CancellationToken cancellationToken)
    {
       await _dbContext.ProductListItem
                    .Where(item => item.ProductListId == listId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task AddProductAsync(ProductListItem productListItem, CancellationToken cancellationToken)
    {
       await _dbContext.ProductListItem.AddAsync(productListItem, cancellationToken);
    }

    public async Task<List<ProductListItem>> GetListItemsByListId(Guid Id, CancellationToken cancellationToken)
    {
        List<ProductListItem>? listItems = await _dbContext.ProductListItem
                        .Where<ProductListItem>(item => item.ProductListId == Id)
                        .ToListAsync(cancellationToken);

        return listItems;
    }

    public async Task<ProductListItem> GetListItemByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
       return await _dbContext.ProductListItem.FindAsync(Id);
    }

    public async Task SetItemVisibilityAsync(ProductListItem productListItem, CancellationToken cancellationToken)
    {
        _dbContext.ProductListItem.Update(productListItem);
    }
    public void RemoveListItem(ProductListItem listItem, CancellationToken cancellationToken)
    {
        _dbContext.ProductListItem.Remove(listItem);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
       await _dbContext.SaveChangesAsync(cancellationToken);
    }

}