namespace Creational.Singleton.Demos;

public sealed class EagerSettingsDemo : IDemo
{
    public string Category => "Singleton";
    public string Name => "Eager (static readonly)";

    public Task RunAsync()
    {
        Console.WriteLine("Before Creating");
        Console.WriteLine(EagerSettings.Instance.Name);
        Console.WriteLine(ReferenceEquals(EagerSettings.Instance, EagerSettings.Instance));
        return Task.CompletedTask;
    }
}
