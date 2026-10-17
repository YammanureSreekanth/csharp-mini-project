namespace Creational.Builder.Demos;

public sealed class InvoiceDirectorDemo : IDemo
{
    public string Category => "Builder";
    public string Name => "GoF builder + director (invoice)";

    public Task RunAsync()
    {
        List<(string, decimal)> lines = [("Navy Suit", 499m), ("Tie", 60m)];
        IInvoiceBuilder textInvoiceBuilder = new TextInvoiceBuilder();
        IInvoiceBuilder htmlInvoiceBuilder = new HtmlInvoiceBuilder();
        Console.WriteLine(new InvoiceDirector(textInvoiceBuilder).Construct("Sreekanth", lines));
        Console.WriteLine(new InvoiceDirector(htmlInvoiceBuilder).Construct("Kranthi", lines));
        return Task.CompletedTask;
    }
}
