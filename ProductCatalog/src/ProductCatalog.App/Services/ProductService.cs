using ProductCatalog.App.Models.Commands;
using ProductCatalog.App.Models.Entities;
using ProductCatalog.App.Repositories.Interfaces;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.App.Services;

public sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public Task<Product> CreateProductAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task<Product> DeleteProductAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Product> GetProductAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Product> UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
