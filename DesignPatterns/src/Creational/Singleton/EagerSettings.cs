namespace Creational.Singleton;

public sealed class EagerSettings
{
    public static readonly EagerSettings Instance = new(); // the CLR guarantees this runs once, thread-safe

    static EagerSettings() { } // empty static constructor: initialise on first use, not earlier

    private EagerSettings()
    {
        Console.WriteLine("EagerSettings created");
    }

    public string Name => "Sreekanth";
}
