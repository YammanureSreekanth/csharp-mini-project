namespace Creational.Singleton;

public sealed class RequestTracer
{
    public int OwnerThread { get; }

    private RequestTracer(int thread)
    {
        OwnerThread = thread;
    }

    private static readonly ThreadLocal<RequestTracer> _perThread = new(() =>
        new RequestTracer(Environment.CurrentManagedThreadId)
    );

    public static RequestTracer Current => _perThread.Value!;
}
