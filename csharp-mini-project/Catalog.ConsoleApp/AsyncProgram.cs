namespace Catalog.ConsoleApp;

public class AsyncProgram
{
    public static async Task<string> DoAsynWork()
    {
        await Task.Delay(1000).ConfigureAwait(true); // Simulate the DB or API Connection

        return "Work completed!";
    }

    public static string DoWork()
    {
        Task.Delay(1000); // mimicing the DB or API Connection

        return "Async Work completed!";
    }

    public static async Task DoWorkAsync(CancellationToken token)
    {
        Console.WriteLine("Starting work");

        // Simulate 5 seconds of work
        for (int i = 0; i < 5; i++)
        {
            token.ThrowIfCancellationRequested(); // Check for cancellation

            await Task.Delay(1000, token);

            Console.WriteLine($"Completed part {i + 1}");
        }

        Console.WriteLine("Work completed successfully.");
    }
     public static async Task Run()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();

        // Console.WriteLine("Hello, World!");

        // Console.WriteLine($"Before: {Thread.CurrentThread.ManagedThreadId}");

        // var result = await DoAsynWork();

        // Console.WriteLine(value: $"After: {Thread.CurrentThread.ManagedThreadId}");

        // Console.WriteLine(result);

        cts.CancelAfter(2000);
        try
        {
            await DoWorkAsync(cts.Token);   
        }
        catch (Exception ex)
        {
             Console.WriteLine($"The Operation is Cancelled {ex}");
        }
    }
}

