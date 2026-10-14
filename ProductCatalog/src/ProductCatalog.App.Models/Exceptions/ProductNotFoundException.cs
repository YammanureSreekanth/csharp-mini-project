namespace ProductCatalog.App.Models.Exceptions;

public class ProductNotFoundException(object key) : Exception($"No Product found for id '{key}'.");
