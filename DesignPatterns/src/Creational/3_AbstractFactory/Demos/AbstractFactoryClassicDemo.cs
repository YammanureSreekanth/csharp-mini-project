namespace Creational.AbstractFactory.Demos;

public sealed class AbstractFactoryClassicDemo : IDemo
{
    public string Category => "AbstractFactory";
    public string Name => "Classic (EU / US checkout)";

    public Task RunAsync()
    {
        Console.WriteLine(new Checkout(new EUFactory()).Run(100m));
        Console.WriteLine(new Checkout(new USFactory()).Run(100m));
        return Task.CompletedTask;
    }
}
