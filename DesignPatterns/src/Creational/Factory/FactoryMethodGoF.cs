namespace Creational.Factory;

public interface IExporter
{
    string Export(string text);
}

public class PDFExporter : IExporter
{
    public string Export(string text) => $"{text} is gettng exported in [PDP]";
}

public class CSVExporter : IExporter
{
    public string Export(string text) => $"{text} is gettng exported in [CSV]";
}

public abstract class ReportJob
{
    protected abstract IExporter CreateExporter(); // here, this is Factory method.

    public string Run(string data) => CreateExporter().Export(data.ToUpperInvariant());
}

public class FinanceReportJob : ReportJob
{
    protected override IExporter CreateExporter() => new PDFExporter();
}

public class WarehouseReportJob : ReportJob
{
    protected override IExporter CreateExporter() => new CSVExporter();
}
