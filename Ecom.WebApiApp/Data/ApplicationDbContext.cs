using Ecom.WebApiApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecom.WebApiApp.Data;

public class ApplicationDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<Product> Products {set; get;}
}