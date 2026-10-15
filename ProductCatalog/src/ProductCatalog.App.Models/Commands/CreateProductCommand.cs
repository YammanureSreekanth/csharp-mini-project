namespace ProductCatalog.App.Models.Commands;

public sealed record CreateProductCommand(
    string Id,
    string Name,
    string Description,
    string CategoryId,
    bool Searchable,
    bool Online,
    decimal Price,
    int ATS
);
