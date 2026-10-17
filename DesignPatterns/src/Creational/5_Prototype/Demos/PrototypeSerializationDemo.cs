namespace Creational.Prototype.Demos;

public class PrototypeSerializationDemo : IDemo
{
    public string Category => "Prototype";

    public string Name => "Prototype Serialization Demo";

    public Task RunAsync()
    {
        Listing? navy = new Listing { Title = "Navy Suit", Tags = { "formal" }, Warehouse = { City = "Amsterdam" } };
        var grey = navy.DeepCopy();
        grey.Tags.Add("sale");
        grey.Warehouse.City = "Rotterdam";

        Console.WriteLine($"{string.Join(",", navy.Tags)} in {navy.Warehouse.City}");
        Console.WriteLine($"{string.Join(",", grey.Tags)} in {grey.Warehouse.City}");

        return Task.CompletedTask;
    }
}