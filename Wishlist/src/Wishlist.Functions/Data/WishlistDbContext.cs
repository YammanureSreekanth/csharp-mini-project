using Microsoft.EntityFrameworkCore;
using Wishlist.Core.Entities;

namespace Wishlist.Functions.Data;

/// <summary>
/// This DI & IoC class which gets the dbContextOptions and creates the DBContext
/// </summary>
/// <param name="dbContextOptions"></param>
public class WishlistDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<ProductList> ProductList {set; get;}
    public DbSet<ProductListItem> ProductListItem {set; get;}
}
