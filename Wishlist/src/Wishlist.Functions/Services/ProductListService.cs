using Wishlist.Core.Contracts;
using Wishlist.Core.Entities;
using Wishlist.Core.Interfaces;

namespace Wishlist.Functions.Services;

public class ProductListService(IProductListRepository repo) : IProductListService
{
    public async Task<IReadOnlyList<ProductList>> GetListsForCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        return await repo.GetByCustomerAsync(customerId, cancellationToken);
    }

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

    public async Task<ProductList?> GetListAsync(Guid listId, CancellationToken cancellationToken)
    {
        ProductList? list = await repo.GetByIdAsync(listId, cancellationToken);
        List<ProductListItem>? items = await repo.GetListItemsByListId(listId, cancellationToken);
        list.Items = items;
        return list;
    }

    public async Task DeleteListAsync(Guid listId, CancellationToken cancellationToken)
    {
        ProductList? list = await repo.GetByIdAsync(listId, cancellationToken);
        repo.Remove(list);
        await repo.SaveChangesAsync(cancellationToken);
        await repo.RemoveItemsByListId(listId, cancellationToken);
    }

    public async Task<ProductList> SetListVisibilityAsync(Guid productListId, bool isPublic, CancellationToken cancellationToken)
    {
        
        ProductList productList = await repo.GetByIdAsync(productListId, cancellationToken);

        productList.IsPublic = isPublic;
        productList.ModifiedDate = DateTime.UtcNow;

        await repo.SetListVisibilityAsync(productList, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken);

        return productList;
    }

    public async Task AddProductAsync(Guid listId, CreateProductListItemRequest req, CancellationToken cancellationToken)
    {
        ProductList? productList = await repo.GetByIdAsync(listId, cancellationToken);

        ProductListItem productListItem = new ProductListItem { 
            Id = Guid.NewGuid(),
            ProductId = req.ProductId,
            List = productList
        };

        productListItem.IsPublic = req.IsPublic;
        productListItem.CreatedDate = DateTime.UtcNow;
        productListItem.ModifiedDate = DateTime.UtcNow;

        await repo.AddProductAsync(productListItem, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductListItem> SetItemVisibilityAsync(Guid listItemId,  bool isPublic, CancellationToken cancellationToken)
    {
        ProductListItem listItem = await repo.GetListItemByIdAsync(listItemId, cancellationToken);
        listItem.IsPublic = isPublic;
        await repo.SetItemVisibilityAsync(listItem, cancellationToken);
        await repo.SaveChangesAsync(cancellationToken);

        return listItem;
    }

    public async Task RemoveListItemAsync(Guid listId, CancellationToken cancellationToken)
    {
        ProductListItem productListItem = await repo.GetListItemByIdAsync(listId, cancellationToken);
        repo.RemoveListItem(productListItem, cancellationToken);
    }
}