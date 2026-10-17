namespace Creational.Builder.Demos;

public sealed class HtmlBuilderDemo : IDemo
{
    public string Category => "Builder";
    public string Name => "Plain builder (HTML list)";

    public Task RunAsync()
    {
        HTMLBuilder htmlBuilder = new();
        htmlBuilder.AddItem("Sreekanth");
        htmlBuilder.AddItem("Aravind");
        string rawHtml = htmlBuilder.Build();
        Console.WriteLine(rawHtml);
        return Task.CompletedTask;
    }
}
