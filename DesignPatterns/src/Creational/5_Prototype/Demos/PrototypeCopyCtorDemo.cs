namespace Creational.Prototype.Demos;

public sealed class PrototypeCopyCtorDemo : IDemo
{
    public string Category => "Prototype";
    public string Name => "Prototype Ctor";

    public Task RunAsync()
    {
        ProductCtor product = new ProductCtor("Navy Suit", "Navy", new AddressForCtor("AMS"));

        ProductCtor cloneProduct = new ProductCtor(product);
        cloneProduct.Address.City = "Almere";
        cloneProduct.Name = "Blue Suit";
        cloneProduct.Color = "Blue Suit";

        Console.WriteLine(product.Name);
        Console.WriteLine(product.Color);
        Console.WriteLine(product.Address.City);

        Console.WriteLine("=============");

        Console.WriteLine(cloneProduct.Name);
        Console.WriteLine(cloneProduct.Color);
        Console.WriteLine(cloneProduct.Address.City);

        return Task.CompletedTask;
    }
}
