using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wishlist.Functions.Interfaces;
using Wishlist.Functions.Services;
using Wishlist.Infrastructure;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

IHost? host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureOpenApi()
    .ConfigureServices((context, services) =>
    {
        
        services.AddInfrastructure(context.Configuration);
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
