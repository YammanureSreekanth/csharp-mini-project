using Microsoft.EntityFrameworkCore;
using ProductCatalog.App.Models.Entities;
using ProductCatalog.App.Repositories.Interfaces;
using ProductCatalog.Infra.Persistance;

namespace ProductCatalog.Infra.Repositories;

public class ProductRepository(ProductDbContext dbContext) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);

        return product;
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
