namespace Creational.Prototype;

public class Address
{
    public string City = "";
}

public class Product : ICloneable
{
    public string Name = "";
    public string Color = "";
    public Address Address = new();
    public object Clone() => MemberwiseClone();
}
