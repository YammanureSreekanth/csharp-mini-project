namespace Creational.Singleton.Demos;

public sealed class LockedSettingsDemo : IDemo
{
    public string Category => "Singleton";
    public string Name => "Double-checked locking";

    public async Task RunAsync()
    {
        Task<LockedSettings>[] tasks = Enumerable
            .Range(0, 10)
            .Select(_ => Task.Run(() => LockedSettings.Instance))
            .ToArray();
        await Task.WhenAll(tasks);
        Console.WriteLine($"Instances are created {LockedSettings.Created}");
    }
}
