using Microsoft.EntityFrameworkCore;
using Wishlist.Core.Entities;

namespace Wishlist.Functions.Data;

public class WishlistDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<ProductList> ProductList {set; get;}
    public DbSet<ProductListItem> ProductListItem {set; get;}
}
