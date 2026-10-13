namespace Creational.AbstractFactory;

public static class RegionFactoryLocator
{
    public static Dictionary<string, IRegionFactory> Discover()
    {
        Type[]? regionFactories = typeof(IRegionFactory).Assembly.GetTypes();
        Dictionary<string, IRegionFactory>? factories = regionFactories
            .Where(f =>
                typeof(IRegionFactory).IsAssignableFrom(f)
                && f is { IsClass: true, IsAbstract: false }
                && f.Namespace == typeof(IRegionFactory).Namespace
            )
            .ToDictionary(
                f => f.Name.Replace("Factory", ""),
                f => (IRegionFactory)Activator.CreateInstance(f)!
            );
        return factories;
    }
}
