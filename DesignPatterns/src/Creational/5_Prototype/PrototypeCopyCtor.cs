namespace Creational.Prototype;

public class AddressForCtor
{
    public string City = "";
    public AddressForCtor (string city)
    {
        City = city;
    } 
    public AddressForCtor (AddressForCtor other) : this(other.City) {}
}

public class ProductCtor
{
    public string Name = "";
    public string Color = "";
    public AddressForCtor Address;
    public ProductCtor (string name, string color, AddressForCtor address)
    {
        Name = name;
        Color = color;
        Address = address;
    }

    public ProductCtor(ProductCtor anotherProduct) : this(anotherProduct.Name, anotherProduct.Color, new AddressForCtor(anotherProduct.Address)) {}
}