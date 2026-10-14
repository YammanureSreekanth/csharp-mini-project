namespace ProductCatalog.App.Models.Entities;

public class SkuPrice
{
    public string SkuId { get; private set; }
    public Money Amount { get; private set; }
}
