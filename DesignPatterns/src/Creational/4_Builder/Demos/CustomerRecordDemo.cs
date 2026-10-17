namespace Creational.Builder.Demos;

public sealed class CustomerRecordDemo : IDemo
{
    public string Category => "Builder";
    public string Name => "Record with-expression";

    public Task RunAsync()
    {
        var sreekanth = new CustomerRecord { Name = "Sreekanth", Email = "test@gmail.com", Tier = "None" };
        var moved = sreekanth with { Shipping = new Address("JAN", "AMS", "1121PP") };

        Console.WriteLine(sreekanth.Shipping is null);
        Console.WriteLine(moved.Shipping?.City);
        return Task.CompletedTask;
    }
}
