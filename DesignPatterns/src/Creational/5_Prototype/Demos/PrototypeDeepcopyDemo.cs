namespace Creational.Prototype.Demos;

public class PrototypeDeepcopyDemo : IDemo
{
    public string Category => "Prototype";

    public string Name => "Prototype Deepcopy";

    public Task RunAsync()
    {
        ProductListing productListing = new ProductListing
        {
            Name = "Navy Suit",
            Color = "Navy",
            Price = 135.9m,
            Address = new Location { City = "AMS", Street = "JAN"}
        };

        ProductListing productListingCopy = productListing.Clone();
        productListingCopy.Name = "Blue Suit";
        productListingCopy.Color = "Blue";
        productListingCopy.Address.City = "Almere";

        Money money = new Money(150, "EUR");
        Money moneyCopy = money with { Price = 139.9m, Curreny = "USD" };

        Console.WriteLine(productListing);
        Console.WriteLine(productListingCopy);

        Console.WriteLine($"Money Price: {money.Price} Currency: {money.Curreny}");
        Console.WriteLine($"Money Copy Price: {moneyCopy.Price} Currency: {moneyCopy.Curreny}");

        return Task.CompletedTask;
    }
}