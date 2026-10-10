namespace Creational.Factory;

public record Order(int Id, string Customer, DateOnly PlacedOn);

public class OrderFactory
{
    private readonly Func<DateOnly> _today;

    public OrderFactory(Func<DateOnly> today)
    {
        _today = today;
    }

    private int OrderId = 1000;

    public Order Create(string Customer) => new Order(++OrderId, Customer, _today());
}
