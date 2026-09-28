namespace Wishlist.Core.Exceptions;

/// <summary>
/// This is expection when ProductList is not found by ListId
/// </summary>
/// <param name="listId"></param>
public class ProductListNotFoundException(Guid listId)
    : Exception($"List {listId} was not found.");

/// <summary>
/// This is exception when customer is trying to add same product
/// </summary>
/// <param name="productId"></param>
public class DuplicateProductException(string productId)
    : Exception($"Product '{productId}' is already in this list.");

/// <summary>
/// This is exception when general error happened
/// </summary>
/// <param name="message"></param>
public class ValidationException(string message) : Exception(message);