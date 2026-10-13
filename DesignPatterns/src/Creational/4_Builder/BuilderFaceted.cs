namespace Creational.Builder;

public class CustomerOrder
{
    public string CustomerName = "";
    public string Email = "";
    public string Street = "";
    public string City = "";
    public string Postcode = "";

    public override string ToString() => $"{CustomerName} | {Email} | {Street}, {Postcode} {City}";
}

public class CustomerFacet : CustomerOrderBuilder
{
    public CustomerFacet(CustomerOrder item)
    {
        Item = item;
    }

    public CustomerFacet Named(string name)
    {
        Item.CustomerName = name;
        return this;
    }

    public CustomerFacet WithEmail(string email)
    {
        Item.Email = email;
        return this;
    }
}

public class ShippingFacet : CustomerOrderBuilder
{
    public ShippingFacet(CustomerOrder item)
    {
        Item = item;
    }

    public ShippingFacet Street(string street)
    {
        Item.Street = street;
        return this;
    }

    public ShippingFacet InCity(string city)
    {
        Item.City = city;
        return this;
    }

    public ShippingFacet WithPostcode(string pc)
    {
        Item.Postcode = pc;
        return this;
    }
}

public class CustomerOrderBuilder
{
    protected CustomerOrder Item = new CustomerOrder();

    public CustomerFacet Customer => new(Item);
    public ShippingFacet Shipping => new(Item);

    public static implicit operator CustomerOrder(CustomerOrderBuilder b) => b.Item; // acts as Build()
}
