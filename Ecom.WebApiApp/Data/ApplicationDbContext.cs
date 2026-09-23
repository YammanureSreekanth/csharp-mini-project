using Ecom.WebApiApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecom.WebApiApp.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions dbContextOptions): base(dbContextOptions)
    {
        
    }
    public DbSet<Product> Products {set; get;}
}