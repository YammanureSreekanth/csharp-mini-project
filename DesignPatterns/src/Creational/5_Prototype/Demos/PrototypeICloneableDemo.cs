namespace Creational.Prototype.Demos;

public sealed class PrototypeICloneableDemo : IDemo
{
    public string Category => "Prototype";
    public string Name => "IClonelable Prototype (Product)";

    public Task RunAsync()
    {
        Product product = new Product();
        product.Name = "Navy Suit";
        product.Color = "Navy";
        product.Address.City = "AMS";

        Product cloneProduct = (Product)product.Clone();
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
