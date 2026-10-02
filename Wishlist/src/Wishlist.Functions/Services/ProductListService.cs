using Wishlist.Core.Contracts;
using Wishlist.Functions.DTOs;
using Wishlist.Core.Entities;
using Wishlist.Core.Exceptions;
using Wishlist.Core.Interfaces;
using Wishlist.Functions.Mapping;
using Wishlist.Functions.Interfaces;

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
    public async Task<IReadOnlyList<ProductListDto>> GetListsForCustomerAsync(string customerId, CancellationToken cancellationToken)
    {   
        IReadOnlyList<ProductList> lists = await repo.GetByCustomerAsync(customerId, cancellationToken);
        
        return lists.Select(ProductListMapper.ToJustListDto).ToList();
    }

    /// <summary>
    /// Creates the ProductList
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductListDto> CreateListAsync(string customerId, CreateListRequest req, CancellationToken cancellationToken)
    {
        ProductList list = new ProductList { Id = Guid.NewGuid(), Name = req.Name, CustomerId = customerId };
        
        list.IsPublic = req.IsPublic;
        
        list.Type = req.Type;
        
        list.CreationDate = DateTime.UtcNow;
        
        list.ModifiedDate = DateTime.UtcNow;

        await repo.AddAsync(list, cancellationToken);
        
        await repo.SaveChangesAsync(cancellationToken);

        ProductListDto listDto = ProductListMapper.ToJustListDto(list);

        return listDto;
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
        
        repo.RemoveList(list);
        
        await repo.SaveChangesAsync(cancellationToken);
        
    }

    /// <summary>
    /// Sets the List Visibility
    /// </summary>
    /// <param name="productListId"></param>
    /// <param name="isPublic"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductListDto> UpdateListAsync(Guid productListId, UpdateListVisibilityRequest request, CancellationToken cancellationToken)
    {
        
        ProductList? productList = await repo.GetByIdAsync(productListId, cancellationToken);

        if (productList is null)
        {
            throw new NotFoundException(nameof(ProductList), productListId);
        }

        if (request.IsPublic.HasValue)
        {
            productList.IsPublic = request.IsPublic.Value;            
        }
        
        productList.ModifiedDate = DateTime.UtcNow;

        await repo.UpdateListAsync(productList, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken);

        return ProductListMapper.ToJustListDto(productList);
    }

    /// <summary>
    /// Gets the list along with items
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductListWithItemsDto>? GetListAsync(Guid listId, CancellationToken cancellationToken)
    {
        ProductList? list = await repo.GetByIdAsync(listId, cancellationToken);

        if (list is null)
        {
            throw new NotFoundException(nameof(ProductList), listId);
        }
        
        List<ProductListItem>? items = await repo.GetListItemsByListId(listId, cancellationToken);
        
        list.Items = items ?? [];
        
        ProductListWithItemsDto productListWithItemsDto = ProductListMapper.ToDto(list);

        return productListWithItemsDto;
    }

    /// <summary>
    /// Add Product to List
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductListItemDto>? AddProductAsync(Guid listId, CreateProductListItemRequest req, CancellationToken cancellationToken)
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

        productListItem.Quantity = req.Quantity;

        productListItem.IsPublic = req.IsPublic;
        
        productListItem.CreatedDate = DateTime.UtcNow;
        
        productListItem.ModifiedDate = DateTime.UtcNow;

        await repo.AddProductAsync(productListItem, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken);

        return ProductListMapper.ToItemDto(productListItem);
    }

    /// <summary>
    /// Sets the item visibility
    /// </summary>
    /// <param name="listItemId"></param>
    /// <param name="isPublic"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductListItemDto>? UpdateListItemAsync(Guid listId, Guid listItemId,  UpdateItemVisibilityRequest request, CancellationToken cancellationToken)
    {   

        ProductList? productList = await repo.GetByIdAsync(listId, cancellationToken);

        if (productList is null)
        {
            throw new NotFoundException(nameof(ProductList), listId);
        }

        ProductListItem? listItem = await repo.GetListItemByIdAsync(listItemId, cancellationToken);

        if (listItem is null)
        {
            throw new NotFoundException(nameof(ProductListItem), listItemId);
        }

        if (request.IsPublic.HasValue)
        {
            listItem.IsPublic = request.IsPublic.HasValue;   
        }

         if (request.Quantity.HasValue)
        {
            listItem.Quantity = request.Quantity.Value;
        }

        listItem.ModifiedDate = DateTime.UtcNow;
        
        await repo.UpdateListItemAsync(listItem, cancellationToken);
        
        await repo.SaveChangesAsync(cancellationToken);

        return ProductListMapper.ToItemDto(listItem);
    }

    /// <summary>
    /// Removes the listItem from List
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveListItemAsync(Guid listId, Guid itemId, CancellationToken cancellationToken)
    {   
        ProductList? productList = await repo.GetByIdAsync(listId, cancellationToken);
        
        if (productList is null)
        {
            throw new NotFoundException(nameof(productList), listId);
        }

        ProductListItem? productListItem = await repo.GetListItemByIdAsync(itemId, cancellationToken);

        if (productListItem is null)
        {
            throw new NotFoundException(nameof(ProductListItem), itemId);
        }
        
        repo.RemoveListItem(productListItem, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken);
    }
}