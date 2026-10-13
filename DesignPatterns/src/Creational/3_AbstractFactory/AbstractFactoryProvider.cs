namespace Creational.AbstractFactory;

public static class RegionFactories
{
    private static readonly Dictionary<string, IRegionFactory> _regionFactories = new Dictionary<
        string,
        IRegionFactory
    >
    {
        ["eu"] = new EUFactory(),
        ["us"] = new USFactory(),
        ["de"] = new EUFactory(),
    };

    public static IRegionFactory For(string country)
    {
        return _regionFactories.TryGetValue(country.ToLowerInvariant(), out var regionFactory)
            ? regionFactory
            : throw new NotSupportedException($"We do not ship to {country} yet");
    }
}
