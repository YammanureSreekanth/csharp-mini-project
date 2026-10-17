namespace Creational.AbstractFactory.Demos;

public sealed class AbstractFactoryReflectionDemo : IDemo
{
    public string Category => "AbstractFactory";
    public string Name => "Reflection discovery";

    public Task RunAsync()
    {
        Dictionary<string, IRegionFactory> regions = RegionFactoryLocator.Discover();
        Console.WriteLine(string.Join(", ", regions.Keys.OrderBy(k => k)));
        Console.WriteLine(regions["US"].PaymentGateway().Pay(50m));
        return Task.CompletedTask;
    }
}
