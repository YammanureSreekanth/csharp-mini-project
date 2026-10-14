using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductCatalog.Infra.Persistance;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ProductDbContext>? optionsBuilder =
            new DbContextOptionsBuilder<ProductDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=FuncApp;User ID=sa;Password=PraticeApp@2031;TrustServerCertificate=True;Encrypt=False;"
        );

        return new ProductDbContext(optionsBuilder.Options);
    }
}
