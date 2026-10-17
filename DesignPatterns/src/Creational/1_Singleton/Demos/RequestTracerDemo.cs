namespace Creational.Singleton.Demos;

public sealed class RequestTracerDemo : IDemo
{
    public string Category => "Singleton";
    public string Name => "Per-thread (ThreadLocal)";

    public Task RunAsync()
    {
        Console.WriteLine($"Checking {nameof(RequestTracer)} singleton");
        RequestTracer mine = RequestTracer.Current;
        Console.WriteLine(ReferenceEquals(mine, RequestTracer.Current)); // same thread, same object
        RequestTracer? other = null;
        Thread thread = new(() => other = RequestTracer.Current);
        thread.Start();
        thread.Join();
        Console.WriteLine(ReferenceEquals(mine, other)); // other thread, own object
        return Task.CompletedTask;
    }
}
