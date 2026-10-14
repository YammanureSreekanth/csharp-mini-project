namespace ProductCatalog.App.Models.Entities;

public class Variant
{
    public string Id { get; private set; }
    public string BaseProductId { get; private set; }
    public Product BaseProduct { get; private set; }
    public bool Online { get; private set; } = false;
    public string AttributeId { get; private set; } = "size";
    public string AttributeValue { get; private set; }
    public bool Searchable { get; private set; } = false;

    public Variant(string id, string attributeId, string attributeValue, Product baseProduct)
    {
        Id = id;
        AttributeId = attributeId;
        AttributeValue = attributeValue;
        BaseProduct = baseProduct;
        BaseProductId = baseProduct.Id;
    }
}
