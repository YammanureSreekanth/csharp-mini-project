namespace Creational.Singleton.Demos;

public sealed class LazySettingsDemo : IDemo
{
    public string Category => "Singleton";
    public string Name => "Lazy<T>";

    public Task RunAsync()
    {
        Console.WriteLine($"Checking {nameof(LazySettings)} singleton");
        LazySettings lazySettings1 = LazySettings.Instance;
        LazySettings lazySettings2 = LazySettings.Instance;
        Console.WriteLine(ReferenceEquals(lazySettings1, lazySettings2));
        Console.WriteLine($"Name1: {lazySettings1.Name} Role1: {lazySettings1.Role}");
        Console.WriteLine($"Name2: {lazySettings2.Name} Role2: {lazySettings2.Role}");
        return Task.CompletedTask;
    }
}
