namespace Creational.Builder;

public class HTMLBuilder
{
    private readonly List<string> _items = new();

    public void AddItem(string item) => _items.Add(item);

    public string Build() => "<ul>" + string.Concat(_items.Select(i => $"<li>{i}<li>")) + "</ul>";
}
