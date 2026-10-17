namespace Creational.Factory.Demos;

public sealed class OrderFactoryDemo : IDemo
{
    public string Category => "Factory";
    public string Name => "Factory class with injected clock (orders)";

    public Task RunAsync()
    {
        OrderFactory orderFactory = new(() => new DateOnly(2026, 10, 4));
        List<string> customers = ["Sreekanth", "Aravind"];
        foreach (string customer in customers)
        {
            Order order = orderFactory.Create(customer);
            Console.WriteLine($"{order.Id} {order.Customer} {order.PlacedOn}");
        }
        return Task.CompletedTask;
    }
}
