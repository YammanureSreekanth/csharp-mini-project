using ProductCatalog.App.Clients.Interfaces;
using ProductCatalog.App.Models;
using ProductCatalog.App.Models.Commands;
using ProductCatalog.App.Models.Entities;
using ProductCatalog.App.Models.Exceptions;
using ProductCatalog.App.Repositories.Interfaces;
using ProductCatalog.App.Services.Interfaces;

namespace ProductCatalog.App.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    IProductImageClientFactory imageClientFactory
) : IProductService
{
    public async Task<Product> CreateProductAsync(
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

    public async Task<ProductWithImage> GetProductAsync(string id, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(id, cancellationToken)
        ?? throw new ProductNotFoundException(id);

        ProductImageInfo? image = await imageClientFactory.Resolve().GetImageAsync(product.Id, cancellationToken);

        return new ProductWithImage(product, image);
    }

    public Task<Product> UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
