using Wishlist.Core.Contracts;
using Wishlist.Core.Entities;
using Wishlist.Core.Exceptions;
using Wishlist.Core.Interfaces;

namespace Wishlist.Functions.Services;

/// <summary>
/// This is services class and gets the repo object via DI
/// </summary>
/// <param name="repo"></param>
public class ProductListService(IProductListRepository repo) : IProductListService
{
    /// <summary>
    /// To get the list by Customer ID
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<ProductList>> GetListsForCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        return await repo.GetByCustomerAsync(customerId, cancellationToken);
    }

    /// <summary>
    /// Creates the ProductList
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets the list along with items
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductList?> GetListAsync(Guid listId, CancellationToken cancellationToken)
    {
        ProductList? list = await repo.GetByIdAsync(listId, cancellationToken);

        if (list is null)
        {
            throw new NotFoundException(nameof(ProductList), listId);
        }
        
        List<ProductListItem>? items = await repo.GetListItemsByListId(listId, cancellationToken);
        
        list.Items = items;
        
        return list;
    }

    /// <summary>
    /// Deletes the List By listId
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteListAsync(Guid listId, CancellationToken cancellationToken)
    {
        ProductList? list = await repo.GetByIdAsync(listId, cancellationToken);

        if (list is null)
        {
            throw new NotFoundException(nameof(ProductList), listId);
        }
        
        repo.Remove(list);
        
        await repo.SaveChangesAsync(cancellationToken);
        
        await repo.RemoveItemsByListId(listId, cancellationToken);
    }

    /// <summary>
    /// Sets the List Visibility
    /// </summary>
    /// <param name="productListId"></param>
    /// <param name="isPublic"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductList> SetListVisibilityAsync(Guid productListId, bool isPublic, CancellationToken cancellationToken)
    {
        
        ProductList? productList = await repo.GetByIdAsync(productListId, cancellationToken);

        if (productList is null)
        {
            throw new NotFoundException(nameof(ProductList), productListId);
        }

        productList.IsPublic = isPublic;
        
        productList.ModifiedDate = DateTime.UtcNow;

        await repo.SetListVisibilityAsync(productList, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken);

        return productList;
    }

    /// <summary>
    /// Add Product to List
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductListItem>? AddProductAsync(Guid listId, CreateProductListItemRequest req, CancellationToken cancellationToken)
    {
        ProductList? productList = await repo.GetByIdAsync(listId, cancellationToken);

        if (productList is null)
        {
            throw new NotFoundException(nameof(ProductList), listId);
        }

        if (await repo.ExistProductIdByListId(productList.Id, req.ProductId, cancellationToken))
        {
            throw new ConflictProductException(req.ProductId);
        }

        ProductListItem productListItem = new ProductListItem {
            Id = Guid.NewGuid(),
            ProductListId = productList.Id,
            ProductId = req.ProductId
        };

        productListItem.IsPublic = req.IsPublic;
        
        productListItem.CreatedDate = DateTime.UtcNow;
        
        productListItem.ModifiedDate = DateTime.UtcNow;

        await repo.AddProductAsync(productListItem, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken);

        return productListItem;
    }

    /// <summary>
    /// Sets the item visibility
    /// </summary>
    /// <param name="listItemId"></param>
    /// <param name="isPublic"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductListItem?> SetItemVisibilityAsync(Guid listItemId,  bool isPublic, CancellationToken cancellationToken)
    {
        ProductListItem? listItem = await repo.GetListItemByIdAsync(listItemId, cancellationToken);

        if (listItem is null)
        {
            throw new NotFoundException(nameof(ProductListItem), listItemId);
        }

        listItem.IsPublic = isPublic;

        listItem.ModifiedDate = DateTime.Now;
        
        await repo.SetItemVisibilityAsync(listItem, cancellationToken);
        
        await repo.SaveChangesAsync(cancellationToken);

        return listItem;
    }

    /// <summary>
    /// Removes the listItem from List
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveListItemAsync(Guid itemId, CancellationToken cancellationToken)
    {
        ProductListItem? productListItem = await repo.GetListItemByIdAsync(itemId, cancellationToken);

        if (productListItem is null)
        {
            throw new NotFoundException(nameof(ProductListItem), itemId);
        }
        
        repo.RemoveListItem(productListItem, cancellationToken);
    }
}