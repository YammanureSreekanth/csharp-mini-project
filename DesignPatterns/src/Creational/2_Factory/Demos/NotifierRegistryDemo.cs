namespace Creational.Factory.Demos;

public sealed class NotifierRegistryDemo : IDemo
{
    public string Category => "Factory";
    public string Name => "Registry-based factory (notifiers)";

    public Task RunAsync()
    {
        NotifierRegistry registry = new NotifierRegistry()
            .Register("email", () => new EmailNotifierRegistry())
            .Register("sms", () => new SMSNotifierRegistry());

        Console.WriteLine(registry.Create("sms").Send("+31 123456789", "Shipped"));

        // Let's register one more channel now
        registry.Register("push", () => new PushNotifierRegistry());

        Console.WriteLine(registry.Create("push").Send("iPhone", "Shipped"));
        return Task.CompletedTask;
    }
}
