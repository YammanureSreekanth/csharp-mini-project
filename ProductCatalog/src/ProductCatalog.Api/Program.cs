using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductCatalog.App.Services;
using ProductCatalog.App.Services.Interfaces;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using ProductCatalog.Infra;

IHost? host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureOpenApi()
    .ConfigureServices((context, services) =>
    {
        
        services.AddInfrastructure(context.Configuration);
        services.AddScoped<IProductService, ProductService>();

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
        {
            services.AddOpenTelemetry()
                .UseFunctionsWorkerDefaults()
                .UseAzureMonitorExporter();
        }
    })
    .Build();

host.Run();

