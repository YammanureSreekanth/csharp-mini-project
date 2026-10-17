namespace Creational.Builder.Demos;

public sealed class OrderBuilderDemo : IDemo
{
    public string Category => "Builder";
    public string Name => "Fluent builder (order)";

    public Task RunAsync()
    {
        Order order = new OrderBuilder()
            .ForCustomer("Sreekanth")
            .AddItem("S1234")
            .AddItem("S23456")
            .AddCoupon("WELCOME10")
            .CreateAddress("Almere")
            .ApplyGiftWrap(true)
            .Build();

        Console.WriteLine($"{order.Customer}: {string.Join(", ", order.Items)} -> {order.Address}");
        return Task.CompletedTask;
    }
}
