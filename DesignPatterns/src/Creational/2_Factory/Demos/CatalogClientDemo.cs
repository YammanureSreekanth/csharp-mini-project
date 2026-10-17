namespace Creational.Factory.Demos;

public sealed class CatalogClientDemo : IDemo
{
    public string Category => "Factory";
    public string Name => "Async factory method (catalog client)";

    public async Task RunAsync()
    {
        CatalogClient catalogClient = await CatalogClient.CreateAsync();
        Console.WriteLine(string.Join(", ", catalogClient.Catagories));
    }
}
