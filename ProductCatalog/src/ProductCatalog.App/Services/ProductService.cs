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
        Product? product = new ProductBuilder()
            .Id(command.Id)
            .Name(command.Name)
            .Description(command.Description)
            .CategoryId(command.CategoryId)
            .Searchable(command.Searchable)
            .Online(command.Online)
            .Price(command.Price)
            .Ats(command.Ats)
            .Build();

        await productRepository.AddAsync(product, cancellationToken);

        return product;
    }

    public async Task<Product> DeleteProductAsync(string id, CancellationToken cancellationToken)
    {
        Product product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ProductNotFoundException(id);

        await productRepository.DeleteAsync(product, cancellationToken);

        return product;
    }

    public async Task<ProductWithImage> GetProductAsync(string id, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(id, cancellationToken)
        ?? throw new ProductNotFoundException(id);

        ProductImageInfo? image = await imageClientFactory.Resolve().GetImageAsync(product.Id, cancellationToken);

        return new ProductWithImage(product, image);
    }

    public async Task<Product> UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        Product product = await productRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new ProductNotFoundException(command.Id);

        product.UpdateDetails(
            command.Name,
            command.Description,
            command.CategoryId,
            command.Searchable,
            command.Online,
            command.Price,
            command.Ats
        );

        return await productRepository.UpdateAsync(product, cancellationToken);
    }
}
