using System.Reflection;
using Creational;

IDemo[] demos = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IDemo).IsAssignableFrom(t))
    .Select(t => (IDemo)Activator.CreateInstance(t)!)
    .OrderBy(d => d.Category)
    .ThenBy(d => d.Name)
    .ToArray();

// dotnet run                              -> list demos
// dotnet run -- CustomerRecordDemo        -> one demo by class name
// dotnet run -- builder                   -> every demo in a category (or whose name matches)
if (args.Length == 0)
{
    Console.WriteLine("Available demos (pass a class name, category or name fragment):");
    foreach (IDemo d in demos)
    {
        Console.WriteLine($"  [{d.Category}] {d.Name}  ({d.GetType().Name})");
    }

    return;
}

foreach (IDemo demo in demos.Where(d => args.Any(a =>
             d.GetType().Name.Equals(a, StringComparison.OrdinalIgnoreCase) ||
             d.Category.Contains(a, StringComparison.OrdinalIgnoreCase) ||
             d.Name.Contains(a, StringComparison.OrdinalIgnoreCase))))
{
    Console.WriteLine($"\n=== [{demo.Category}] {demo.Name} ===");
    await demo.RunAsync();
}
