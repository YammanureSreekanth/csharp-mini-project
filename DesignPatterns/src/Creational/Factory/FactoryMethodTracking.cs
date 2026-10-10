namespace Creational.Factory;

public class Badge
{
    public string Color { get; set; }
    public string Label { get; set; }

    public Badge(string color, string label)
    {
        Color = color;
        Label = label;
    }
}

public class BadgeFactory
{
    private readonly List<WeakReference<Badge>> _traked = new List<WeakReference<Badge>>();

    public Badge Create(string color, string label)
    {
        Badge badge = new Badge(color, label);
        _traked.Add(new WeakReference<Badge>(badge));
        return badge;
    }

    public void ReColorAll(string color)
    {
        foreach (WeakReference<Badge> weak in _traked)
        {
            if (weak.TryGetTarget(out Badge? badge))
            {
                badge.Color = color;
            }
        }
    }
}
