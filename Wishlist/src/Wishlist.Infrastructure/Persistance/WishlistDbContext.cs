using Microsoft.EntityFrameworkCore;
using Wishlist.Core.Entities;

namespace Wishlist.Infrastructure.Persistence;

public class WishlistDbContext(DbContextOptions<WishlistDbContext> options) : DbContext(options)
{
    public DbSet<ProductList> ProductLists => Set<ProductList>();
    public DbSet<ProductListItem> ProductListItems => Set<ProductListItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WishlistDbContext).Assembly);
    }
}