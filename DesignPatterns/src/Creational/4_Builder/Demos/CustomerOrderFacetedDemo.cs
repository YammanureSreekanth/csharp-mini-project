namespace Creational.Builder.Demos;

public sealed class CustomerOrderFacetedDemo : IDemo
{
    public string Category => "Builder";
    public string Name => "Faceted builder (customer order)";

    public Task RunAsync()
    {
        CustomerOrder order = new CustomerOrderBuilder()
            .Customer.Named("Sreekanth")
            .WithEmail("sreekanth@email.com")
            .Shipping.Street("Jan 56")
            .InCity("Almere")
            .WithPostcode("1211 RP");

        Console.WriteLine(order);
        return Task.CompletedTask;
    }
}
