namespace ProductCatalog.Api.Models.Requests.Product;
public sealed record UpdateProductRequest(
    string Id,
    string Name,
    string Description,
    string CategoryId,
    bool Searchable,
    bool Online,
    decimal Price,
    int ATS);
