namespace Creational.Prototype;

public class Location
{
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    public override string ToString() => $"City: {City}, Street: {Street}";
}

public class ProductListing
{
    public string Name {get; set;}
    public string Color {get; set;}
    public decimal Price {get; set;}
    public Location Address = new Location();

    public override string ToString() => $"Name: {Name}, Color: {Color}, Price: {Price}, Location: => {Address} ";
    public ProductListing Clone() => new ProductListing()
    {
        Name = Name,
        Color = Color,
        Price = Price,
        Address = new Location { Street = Address.Street, City = Address.City }
    };
}

public record Money(decimal Price, string Curreny);