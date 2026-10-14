using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.App.Repositories.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
