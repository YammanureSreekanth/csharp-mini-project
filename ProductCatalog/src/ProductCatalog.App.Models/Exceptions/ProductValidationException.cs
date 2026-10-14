namespace ProductCatalog.App.Models.Exceptions;

public class ProductValidationException(string message) : Exception(message);
