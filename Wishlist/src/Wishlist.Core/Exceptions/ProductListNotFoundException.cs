namespace Wishlist.Core.Exceptions;

public class ProductListNotFoundException(Guid listId)
    : Exception($"List {listId} was not found.");

public class DuplicateProductException(string productId)
    : Exception($"Product '{productId}' is already in this list.");

public class ValidationException(string message) : Exception(message);