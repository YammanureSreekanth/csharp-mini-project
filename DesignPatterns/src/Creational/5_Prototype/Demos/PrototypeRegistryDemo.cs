namespace Creational.Prototype.Demos;

public class PrototypeRegistryDemo : IDemo
{
    public string Category => "Prototype";

    public string Name => "Prototype Registry Demo";

    public Task RunAsync()
    {
        ListingTemplates? templates = new ListingTemplates();
        templates.Register("suit", new ListingRegistry { Title = "Suit", Price = 499m, Tags = { "formal", "wool" } });

        ListingRegistry? navy = templates.Create("suit");
        navy.Title = "Navy Suit";
        
        ListingRegistry? grey = templates.Create("suit");
        grey.Title = "Grey Suit";
        grey.Price = 449m;

        Console.WriteLine($"{navy.Title} {navy.Price}  |  {grey.Title} {grey.Price}  |  template is still {templates.Create("suit").Title}");

        return Task.CompletedTask;
    }
}