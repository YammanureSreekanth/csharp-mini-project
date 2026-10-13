namespace Creational.Builder;

public class Product
{
    public string Name = "";
    public decimal Price;
    public string Fabric = "";

    public override string ToString() => $"{Name} | {Price} | {Fabric}";
}

public abstract class ProductBuilder<TSelf>
    where TSelf : ProductBuilder<TSelf>
{
    protected Product Item = new();

    public TSelf Named(string name)
    {
        Item.Name = name;
        return (TSelf)this;
    }

    public TSelf SetPrice(decimal price)
    {
        Item.Price = price;
        return (TSelf)this;
    }

    // No implmenation in common abstract class
    // public TSelf AddFabric(string fabric)
    // {
    //     Item.Fabric = $"[BASE] | {fabric}";
    //     return (TSelf)this;
    // }

    public Product Build() => Item;
}

public class SuitBuilder : ProductBuilder<SuitBuilder>
{
    public SuitBuilder AddFabric(string fabric)
    {
        Item.Fabric = fabric;
        return this;
    }
}
