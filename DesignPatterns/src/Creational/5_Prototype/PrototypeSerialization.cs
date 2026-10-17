using System.Text.Json;

namespace Creational.Prototype;


public class AddressSerial { 
    public string City { get; set; } = ""; 
}

public class Listing
{
    public string Title { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public AddressSerial Warehouse { get; set; } = new();
}

public static class DeepCopyExtensions
{
    public static T DeepCopy<T>(this T source) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source)); 
}