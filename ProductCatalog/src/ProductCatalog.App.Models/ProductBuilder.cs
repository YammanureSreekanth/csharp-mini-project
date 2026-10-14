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
    private readonly List<Tuple<string, string, string>> _variants = new();

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

    public ProductBuilder AddVariant(string id, string attributeId, string attributeValue)
    {
        Tuple<string, string, string>? variant = new Tuple<string, string, string>(
            id,
            attributeId,
            attributeValue
        );
        _variants.Add(variant);
        return this;
    }

    public Product Build()
    {
        Product product = new Product(_id, _name, _description, _categoryId, _searchable, _online);
        foreach (var (id, attributeId, attributeValue) in _variants)
        {
            product.Variants.Add(new Variant(id, attributeId, attributeValue, product));
        }

        return product;
    }
}
