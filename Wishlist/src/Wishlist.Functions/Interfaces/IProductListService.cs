using Wishlist.Core.Contracts;
using Wishlist.Functions.DTOs;

namespace Wishlist.Functions.Interfaces;

/// <summary>
/// This is Interface
/// </summary>
public interface IProductListService
{
    Task<ProductListDto> CreateListAsync(string customerId, CreateListRequest req, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductListDto>> GetListsForCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<ProductListWithItemsDto>? GetListAsync(Guid listId, CancellationToken cancellationToken);
    Task DeleteListAsync(Guid listId, CancellationToken cancellationToken);
    Task<ProductListDto> UpdateListAsync(Guid productListId, UpdateListVisibilityRequest request, CancellationToken cancellationToken);
    Task<ProductListItemDto>? AddProductAsync(Guid Id, CreateProductListItemRequest req, CancellationToken cancellationToken);
    Task RemoveListItemAsync(Guid listId, Guid itemId, CancellationToken cancellationToken);
    Task<ProductListItemDto> UpdateListItemAsync(Guid listId, Guid itemId, UpdateItemVisibilityRequest request, CancellationToken cancellationToken);
}