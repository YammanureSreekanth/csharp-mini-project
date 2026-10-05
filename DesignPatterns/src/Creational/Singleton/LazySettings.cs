namespace Creational.Singleton;

public sealed class LazySettings
{
    // created on first use, thread-safe
    private static readonly Lazy<LazySettings> _instance = new(() => new LazySettings());

    public static LazySettings Instance => _instance.Value;

    public string Name => "Sreekanth";

    public string Role => "Developer";

    private LazySettings() { } // nobody else can call new
}
