namespace Creational.Singleton.Demos;

public sealed class NaiveSettingsDemo : IDemo
{
    public string Category => "Singleton";
    public string Name => "Naive (not thread-safe)";

    public async Task RunAsync()
    {
        Task<NaiveSettings>[] tasks = Enumerable
            .Range(0, 8)
            .Select(_ => Task.Run(() => NaiveSettings.Instance))
            .ToArray();
        await Task.WhenAll(tasks);
        Console.WriteLine($"Instances created: {NaiveSettings.Created}");
    }
}
