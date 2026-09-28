using Microsoft.EntityFrameworkCore;
using Wishlist.Core.Entities;
using Wishlist.Core.Interfaces;
using Wishlist.Functions.Data;

namespace Wishlist.Functions.Repositories;

/// <summary>
/// This is repository class which get the dbContext object via DI
/// </summary>
/// <param name="dbContext"></param>
public class ProductListRepository(WishlistDbContext dbContext) : IProductListRepository
{
    private readonly WishlistDbContext _dbContext = dbContext;

    /// <summary>
    /// Get all lists by CustomerId
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<ProductList>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        List<ProductList>? results = await _dbContext.ProductList
            .Where(l => l.CustomerId == customerId)
            .OrderBy(l => l.CreationDate)
            .ToListAsync(cancellationToken);

        return results;
    }

    /// <summary>
    /// To add the productlist
    /// </summary>
    /// <param name="list"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddAsync(ProductList list, CancellationToken cancellationToken)
    {
        await _dbContext.ProductList.AddAsync(list, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// To Get the productlist by ListId
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductList?> GetByIdAsync(Guid listId, CancellationToken cancellationToken)
    {
       ProductList? list = await _dbContext.ProductList.FindAsync(listId);

       return list;
    }

    /// <summary>
    /// Sets the Visibility to List
    /// </summary>
    /// <param name="productList"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SetListVisibilityAsync(ProductList productList, CancellationToken cancellationToken)
    {
        _dbContext.ProductList.Update(productList);
    }

    /// <summary>
    /// Removes the ProductList
    /// </summary>
    /// <param name="list"></param>
    public void Remove(ProductList list)
    {
       _dbContext.ProductList.Remove(list);
    }

    /// <summary>
    /// Removes the items by ListId
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveItemsByListId(Guid listId, CancellationToken cancellationToken)
    {
       await _dbContext.ProductListItem
                    .Where(item => item.ProductListId == listId).ExecuteDeleteAsync(cancellationToken);
    }

    /// <summary>
    /// Adds the product to list
    /// </summary>
    /// <param name="productListItem"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddProductAsync(ProductListItem productListItem, CancellationToken cancellationToken)
    {
       await _dbContext.ProductListItem.AddAsync(productListItem, cancellationToken);
    }

    /// <summary>
    /// Gets the listItems by ListId
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<ProductListItem>> GetListItemsByListId(Guid Id, CancellationToken cancellationToken)
    {
        List<ProductListItem>? listItems = await _dbContext.ProductListItem
                        .Where<ProductListItem>(item => item.ProductListId == Id)
                        .ToListAsync(cancellationToken);

        return listItems;
    }

    /// <summary>
    /// Gets the listItem by ItemId
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductListItem?> GetListItemByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
       return await _dbContext.ProductListItem.FindAsync(Id);
    }

    /// <summary>
    /// Tells about if same product is already added into given listId
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ExistProductIdByListId(Guid listId, string productId, CancellationToken cancellationToken)
    {
       return await _dbContext.ProductListItem.AnyAsync(i => i.ProductListId == listId && i.ProductId == productId, cancellationToken);
    }

    /// <summary>
    /// Sets the Item visibility
    /// </summary>
    /// <param name="productListItem"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SetItemVisibilityAsync(ProductListItem productListItem, CancellationToken cancellationToken)
    {
        _dbContext.ProductListItem.Update(productListItem);
    }

    /// <summary>
    /// Removes the ListItem
    /// </summary>
    /// <param name="listItem"></param>
    /// <param name="cancellationToken"></param>
    public void RemoveListItem(ProductListItem listItem, CancellationToken cancellationToken)
    {
        _dbContext.ProductListItem.Remove(listItem);
    }

    /// <summary>
    /// Save changes to DB
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
       await _dbContext.SaveChangesAsync(cancellationToken);
    }

}