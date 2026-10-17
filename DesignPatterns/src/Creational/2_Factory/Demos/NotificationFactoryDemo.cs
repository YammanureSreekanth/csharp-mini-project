namespace Creational.Factory.Demos;

public sealed class NotificationFactoryDemo : IDemo
{
    public string Category => "Factory";
    public string Name => "Simple factory (notifications)";

    public Task RunAsync()
    {
        NotificationFactory.Create("EMAIL").Send("Sreekanth", "Your order is placed");
        return Task.CompletedTask;
    }
}
