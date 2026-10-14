using ProductCatalog.App.Models.Commands;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.App.Services.Interfaces;

public interface IProductService
{
    Task<Product> CreateProductAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken
    );
}
