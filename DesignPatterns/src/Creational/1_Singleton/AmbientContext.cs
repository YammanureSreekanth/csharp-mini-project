namespace Creational.Singleton;

public sealed class AmbientContext : IDisposable
{
    private static readonly AsyncLocal<Stack<decimal>> _taxRates = new();

    private static Stack<decimal> Rates => _taxRates.Value ??= new Stack<decimal>(new[] { 0.5m });

    public static decimal CurrentRate => Rates.Peek();

    public AmbientContext(decimal rate)
    {
        Rates.Push(rate);
    }

    public void Dispose() => Rates.Pop();
}
