namespace Creational.Singleton.Demos;

public sealed class MonostateDemo : IDemo
{
    public string Category => "Singleton";
    public string Name => "Monostate";

    public Task RunAsync()
    {
        Console.WriteLine($"Checking {nameof(Monostate)} singleton");
        Monostate monostate1 = new();
        Console.WriteLine($"Before Price1: {monostate1.Currency}{monostate1.ShopPrice}");
        Monostate monostate2 = new();
        Console.WriteLine($"Before Price2: {monostate2.Currency}{monostate2.ShopPrice}");
        monostate1.Currency = "USD";
        Console.WriteLine($"After Price1: {monostate1.Currency}{monostate1.ShopPrice}");
        Console.WriteLine($"After Price2: {monostate2.Currency}{monostate2.ShopPrice}");
        Console.WriteLine(ReferenceEquals(monostate1, monostate2));
        return Task.CompletedTask;
    }
}
