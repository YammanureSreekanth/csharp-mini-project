namespace Creational.Factory.Demos;

public sealed class PriceFactoryDemo : IDemo
{
    public string Category => "Factory";
    public string Name => "Static factory methods (Price)";

    public Task RunAsync()
    {
        Console.WriteLine(Price.FromGross(10, 2));
        Console.WriteLine(Price.FromNet(3.33m, 2));
        return Task.CompletedTask;
    }
}
