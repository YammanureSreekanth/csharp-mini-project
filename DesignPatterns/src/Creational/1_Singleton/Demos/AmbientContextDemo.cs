namespace Creational.Singleton.Demos;

public sealed class AmbientContextDemo : IDemo
{
    public string Category => "Singleton";
    public string Name => "Ambient context (AsyncLocal)";

    public Task RunAsync()
    {
        Console.WriteLine($"Checking {nameof(AmbientContext)} singleton");
        Console.WriteLine($"Current TaxRate: {AmbientContext.CurrentRate}");
        using (new AmbientContext(1.5m))
        {
            Console.WriteLine($"Inside First USING Current TaxRate: {AmbientContext.CurrentRate}");
            using (new AmbientContext(2.5m))
            {
                Console.WriteLine($"Nested USING Current TaxRate: {AmbientContext.CurrentRate}");
            }
            Console.WriteLine($"After Nested USING Current TaxRate: {AmbientContext.CurrentRate}");
        }
        Console.WriteLine($"Outside USING Current TaxRate: {AmbientContext.CurrentRate}");
        return Task.CompletedTask;
    }
}
