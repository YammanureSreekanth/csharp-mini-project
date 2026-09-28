namespace Wishlist.Core.Exceptions;

/// <summary>
/// This is expection when ProductList or ListItem is not found by Id
/// </summary>
/// <param name="entityName"></param>
/// <param name="key"></param>
public class NotFoundException(string entityName, object key) : Exception($"No {entityName} found for id '{key}'.");

/// <summary>
/// This is exception when customer is trying to add same product
/// </summary>
/// <param name="productId"></param>
public class ConflictProductException(string productId)
    : Exception($"Product '{productId}' is already in this list.");

/// <summary>
/// This is exception when general error happened
/// </summary>
/// <param name="message"></param>
public class ValidationException(string message) : Exception(message);