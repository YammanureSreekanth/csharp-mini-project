namespace ProductCatalog.Api.Models.Responses.Product;

public sealed record ProductResponse(
    string Id,
    string Name,
    string Description,
    string CategoryId,
    bool Searchable,
    bool Online,
    decimal Price,
    int ATS);
