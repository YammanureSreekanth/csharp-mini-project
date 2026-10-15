using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.App.Models;

public class ProductBuilder
{
    private string _id = "";
    private string _name = "";
    private string _description = "";
    private string _categoryId = "";
    private bool _searchable = true;
    private bool _online = true;
    private decimal _price;
    private int _ats;

    public ProductBuilder Id(string id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder Description(string description)
    {
        _description = description;
        return this;
    }

    public ProductBuilder CategoryId(string categoryId)
    {
        _categoryId = categoryId;
        return this;
    }

    public ProductBuilder Searchable(Boolean searchable)
    {
        _searchable = searchable;
        return this;
    }

    public ProductBuilder Online(bool online)
    {
        _online = online;
        return this;
    }

    public ProductBuilder Price(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder Ats(int ats)
    {
        _ats = ats;
        return this;
    }

    public Product Build()
    {
        Product product = new Product(_id, _name, _description, _categoryId, _searchable, _online, _price, _ats);
        
        return product;
    }
}
