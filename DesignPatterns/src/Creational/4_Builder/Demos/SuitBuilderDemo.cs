namespace Creational.Builder.Demos;

public sealed class SuitBuilderDemo : IDemo
{
    public string Category => "Builder";
    public string Name => "CRTP builder (suit)";

    public Task RunAsync()
    {
        Product suit = new SuitBuilder().Named("Suit").SetPrice(99m).AddFabric("wool").Build();
        Console.WriteLine(suit);
        return Task.CompletedTask;
    }
}
