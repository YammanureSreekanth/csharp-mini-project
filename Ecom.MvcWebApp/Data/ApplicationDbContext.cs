using Ecom.MvcWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecom.MvcWebApp.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        
    }
    public DbSet<Employee> Employees {get; set;}
}