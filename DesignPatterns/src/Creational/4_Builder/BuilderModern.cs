namespace Creational.Builder;

public record Address(string Street, string City, string Postcode);

public record CustomerRecord
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string Tier { get; init; } = "Standard";
    public Address? Shipping { get; init; }
}