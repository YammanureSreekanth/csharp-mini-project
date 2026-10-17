using Microsoft.Extensions.DependencyInjection;

namespace Creational.AbstractFactory.Demos;

public sealed class AbstractFactoryDIDemo : IDemo
{
    public string Category => "AbstractFactory";
    public string Name => "Dependency injection";

    public Task RunAsync()
    {
        string configuredRegion = "EU";
        ServiceCollection services = new();
        if (configuredRegion == "EU")
        {
            services.AddSingleton<IRegionFactory, EUFactory>();
        }
        else
        {
            services.AddSingleton<IRegionFactory, USFactory>();
        }
        services.AddScoped<RegionalCheckout>();
        using ServiceProvider provider = services.BuildServiceProvider();
        Console.WriteLine(provider.GetRequiredService<RegionalCheckout>().Run(100m));
        return Task.CompletedTask;
    }
}
