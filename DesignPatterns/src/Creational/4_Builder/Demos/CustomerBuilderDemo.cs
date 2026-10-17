namespace Creational.Builder.Demos;

public sealed class CustomerBuilderDemo : IDemo
{
    public string Category => "Builder";
    public string Name => "Functional builder (customer)";

    public Task RunAsync()
    {
        Customer customer = new CustomerBuilder()
            .Called("Sreekanth")
            .WithEmail("sreekanth@email.com")
            .InTier("None")
            .Build();

        Console.WriteLine(customer);
        return Task.CompletedTask;
    }
}
