namespace Creational.Factory;

public sealed class Price
{
    public decimal Net { get; }
    public decimal Gross { get; }

    private Price(decimal net, decimal gross)
    {
        Net = net;
        Gross = gross;
    }

    public static Price FromNet(decimal net, decimal vatRate) =>
        new(net, Math.Round(net * (1 + vatRate), 2));

    public static Price FromGross(decimal gross, decimal vatRate) =>
        new(Math.Round(gross / (1 + vatRate), 2), gross);

    public override string ToString() => $"net {Net:0.00}, gross {Gross:0.00}";
}
