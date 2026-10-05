namespace Creational.Singleton;

public class NaiveSettings
{
    private static NaiveSettings? _instance;

    public static int Created; // counts how many objects were really made

    private NaiveSettings()
    {
        Interlocked.Increment(ref Created);
        Thread.Sleep(10); // slow constructor makes the race easy to see
    }

    public static NaiveSettings Instance
    {
        get
        {
            if (_instance is null) // two threads can both pass this check
            {
                _instance = new NaiveSettings();
            }

            return _instance;
        }
    }
}