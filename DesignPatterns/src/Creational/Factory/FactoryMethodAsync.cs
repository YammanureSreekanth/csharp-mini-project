namespace Creational.Factory;

public class CatalogClient
{
    public IReadOnlyList<string> Catagories { get; private set; } = Array.Empty<string>();

    private CatalogClient() { }

    private async Task<CatalogClient> InitAsync()
    {
        await Task.Delay(50);
        Catagories = new[] { "Suits", "Jackets", "Trourser" };
        return this;
    }

    public static Task<CatalogClient> CreateAsync() => new CatalogClient().InitAsync();
}
