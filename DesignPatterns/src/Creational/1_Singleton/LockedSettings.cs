namespace Creational.Singleton;

public class LockedSettings
{
    private static volatile LockedSettings? _instance;

    private static readonly object _gate = new();

    public static int Created; // counts how many objects were really made

    private LockedSettings ()
    {
        Interlocked.Increment(ref Created);
    }

    public static LockedSettings Instance
    {
        get
        {
            if (_instance is null)
            {
                lock(_gate)
                {
                    if (_instance is null)
                    {
                        _instance = new LockedSettings();
                    }
                }
            }
            return _instance;
        }
    }
}