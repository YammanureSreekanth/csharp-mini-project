namespace Creational.Prototype;

public class ListingRegistry
{
    public string Title { get; set; } = "";
    public decimal Price { get; set; }
    public List<string> Tags { get; set; } = new();

    public ListingRegistry Clone() => new() { Title = Title, Price = Price, Tags = new List<string>(Tags) };
}

public class ListingTemplates
{
    public Dictionary<string, ListingRegistry> _prototypes = new();

    public void Register(string key, ListingRegistry prototype) => _prototypes.Add(key, prototype);

    public ListingRegistry Create(string key) => _prototypes[key].Clone();
}