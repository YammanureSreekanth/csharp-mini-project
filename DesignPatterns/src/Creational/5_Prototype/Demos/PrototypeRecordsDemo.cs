using System.Collections.Immutable;

namespace Creational.Prototype.Demos;

public class PrototypeRecordsDemo : IDemo
{
    public string Category => "Prototype";

    public string Name => "Prototype Records Demo";

    public Task RunAsync()
    {
        ListingRecord? navy = new ListingRecord("Navy Suit", 499m, ImmutableList.Create("formal"));

        ListingRecord? grey = navy with { Name = "Grey Suit", Price = 599m, Tags = navy.Tags.Add("Sale")};

        Console.WriteLine($"{navy.Name}: {navy.Tags.Count} tag(s)");
        Console.WriteLine($"{grey.Name}: {grey.Tags.Count} tag(s)");

        return Task.CompletedTask;
    }
}