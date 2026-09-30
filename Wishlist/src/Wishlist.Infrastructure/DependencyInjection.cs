using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wishlist.Core.Interfaces;
using Wishlist.Infrastructure.Persistence;
using Wishlist.Infrastructure.Repositories;

namespace Wishlist.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DbCon")
            ?? throw new InvalidOperationException("Missing DB connection string.");

        services.AddDbContext<WishlistDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IProductListRepository, ProductListRepository>();

        return services;
    }
}