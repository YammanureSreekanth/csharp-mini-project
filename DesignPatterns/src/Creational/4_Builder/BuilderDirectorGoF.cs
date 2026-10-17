using System.Text;

namespace Creational.Builder;

public interface IInvoiceBuilder
{
    void AddHeader(string customer);
    void AddLine(string product, decimal price);
    void AddTotal(decimal total);
    string GetResult(); 
}

public class TextInvoiceBuilder : IInvoiceBuilder
{
    private readonly StringBuilder _sb = new StringBuilder();
    public void AddHeader(string customer)
    {
        _sb.AppendLine($"Invoice for Customer: {customer}");
    }

    public void AddLine(string product, decimal price) => _sb.AppendLine($"  {product}: {price:0.00}");

    public void AddTotal(decimal total) =>  _sb.AppendLine($"TOTAL {total:0.00}");

    public string GetResult() => _sb.ToString().TrimEnd();
}

public class HtmlInvoiceBuilder : IInvoiceBuilder
{
    private readonly StringBuilder _sb = new StringBuilder();
    public void AddHeader(string customer)
    {
        _sb.AppendLine($"<h1>Invoice for Customer: {customer}</h1><ul>");
    }

    public void AddLine(string product, decimal price)
    {
       _sb.Append($"<li>{product}: {price:0.00}</li>");
    }

    public void AddTotal(decimal total)
    {
       _sb.Append($"</ul><b>TOTAL {total:0.00}</b>");
    }

    public string GetResult() => _sb.ToString();
}

public class InvoiceDirector(IInvoiceBuilder builder)
{
    public string Construct(string customer, IReadOnlyList<(string, decimal)> lines)
    {
        builder.AddHeader(customer);
        foreach (var (product, price) in lines)
        {
            builder.AddLine(product, price);
        }
        
        builder.AddTotal(lines.Sum(l => l.Item2));

        return builder.GetResult();
    }
}