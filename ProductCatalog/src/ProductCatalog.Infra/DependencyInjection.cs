using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.App.Repositories.Interfaces;
using ProductCatalog.Infra.Persistance;
using ProductCatalog.Infra.Repositories;

namespace ProductCatalog.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string connectionString =
            configuration["SqlConnectionString"]
            ?? throw new InvalidOperationException("Missing DB connection string.");

        services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
