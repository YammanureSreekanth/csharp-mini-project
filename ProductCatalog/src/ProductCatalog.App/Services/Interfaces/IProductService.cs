using ProductCatalog.App.Models;
using ProductCatalog.App.Models.Commands;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.App.Services.Interfaces;

public interface IProductService
{
    Task<Product> CreateProductAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken
    );

    Task<ProductWithImage> GetProductAsync(
        string id,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyCollection<Product>> GetAllProductsAsync(
        CancellationToken cancellationToken
    );

    Task<Product> UpdateProductAsync(
        UpdateProductCommand command,
        CancellationToken cancellationToken
    );

    Task<Product> DeleteProductAsync(
        string id,
        CancellationToken cancellationToken
    );
}
