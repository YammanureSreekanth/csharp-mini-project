using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Wishlist.Infrastructure.Persistence;

namespace Wishlist.Functions;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WishlistDbContext>
{
    public WishlistDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WishlistDbContext>();
        
        // Placeholder only: dotnet-ef never opens a real connection for migrations add/script,
        // it just needs a syntactically valid string to satisfy UseSqlServer(). See docs/LEARNINGS.md.
        optionsBuilder.UseSqlServer("Server=localhost;Database=FuncApp;User ID=sa;Password=PraticeApp@2031;TrustServerCertificate=True;Encrypt=False;");

        return new WishlistDbContext(optionsBuilder.Options);
    }
}