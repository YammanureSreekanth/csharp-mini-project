namespace Creational.AbstractFactory.Demos;

public sealed class AbstractFactoryProviderDemo : IDemo
{
    public string Category => "AbstractFactory";
    public string Name => "Provider lookup by country";

    public Task RunAsync()
    {
        IRegionFactory region = RegionFactories.For("EU");
        Console.WriteLine(region.TaxCalculator().TaxFor(10m));
        return Task.CompletedTask;
    }
}
