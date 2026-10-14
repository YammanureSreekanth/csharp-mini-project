using ProductCatalog.App.Models.Entities;
using ProductCatalog.App.Repositories.Interfaces;

namespace ProductCatalog.Infra.Repositories;

public class ProductRepository : IProductRepository
{
    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
