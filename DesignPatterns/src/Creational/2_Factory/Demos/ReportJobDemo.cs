namespace Creational.Factory.Demos;

public sealed class ReportJobDemo : IDemo
{
    public string Category => "Factory";
    public string Name => "GoF factory method (report jobs)";

    public Task RunAsync()
    {
        ReportJob[] reportJobs = [new FinanceReportJob(), new WarehouseReportJob()];
        foreach (ReportJob job in reportJobs)
        {
            Console.WriteLine(job.Run("Sreekanth"));
        }
        return Task.CompletedTask;
    }
}
