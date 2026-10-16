namespace ProductCatalog.App.Models.Entities;

public class Product
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string CategoryId { get; private set; }
    public bool Searchable { get; private set; } = false;
    public bool Online { get; private set; } = false;
    public decimal Price { get; private set; }
    public int Ats {get; private set; }

    public Product(
        string id,
        string name,
        string description,
        string categoryId,
        bool searchable,
        bool online,
        decimal price,
        int ats
    )
    {
        Id = id;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        Searchable = searchable;
        Online = online;
        Price = price;
        Ats = ats;
    }

    public void UpdateDetails(
        string name,
        string description,
        string categoryId,
        bool searchable,
        bool online,
        decimal price,
        int ats
    )
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        Searchable = searchable;
        Online = online;
        Price = price;
        Ats = ats;
    }
}
