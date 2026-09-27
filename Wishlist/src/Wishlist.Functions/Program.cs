using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wishlist.Core.Interfaces;
using Wishlist.Functions.Services;
using Wishlist.Functions.Data;
using Wishlist.Functions.Repositories;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

IHost? host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureOpenApi()
    .ConfigureServices((context, services) =>
    {
        string? connectionString = context.Configuration["SqlConnectionString"];

        services.AddDbContext<WishlistDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProductListRepository, ProductListRepository>();
        services.AddScoped<IProductListService, ProductListService>();

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
        {
            services.AddOpenTelemetry()
                .UseFunctionsWorkerDefaults()
                .UseAzureMonitorExporter();
        }
    })
    .Build();

host.Run();
