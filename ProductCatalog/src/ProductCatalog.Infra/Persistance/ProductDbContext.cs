using Microsoft.EntityFrameworkCore;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Infra.Persistance;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SkuInventory> SkuInventories => Set<SkuInventory>();
    public DbSet<SkuPrice> SkuPrices => Set<SkuPrice>();
    public DbSet<Variant> Variants => Set<Variant>();
    public DbSet<SellableSku> SellableSkus => Set<SellableSku>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
    }
}
