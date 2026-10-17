namespace Creational.Factory.Demos;

public sealed class BadgeFactoryDemo : IDemo
{
    public string Category => "Factory";
    public string Name => "Tracking factory (badges, WeakReference)";

    public Task RunAsync()
    {
        BadgeFactory badgeFactory = new();
        Badge shirt = badgeFactory.Create("GREEN", "SHIRT");
        Badge trouser = badgeFactory.Create("BLUE", "TROUSER");
        Console.WriteLine($"[SHIRT] Color: {shirt.Color} Label {shirt.Label}");
        Console.WriteLine($"[TROUSER] Color: {trouser.Color} Label {trouser.Label}");
        badgeFactory.ReColorAll("YELLOW");
        Console.WriteLine($"After changing [SHIRT] Color: {shirt.Color} Label {shirt.Label}");
        Console.WriteLine($"After changing [TROUSER] Color: {trouser.Color} Label {trouser.Label}");
        return Task.CompletedTask;
    }
}
