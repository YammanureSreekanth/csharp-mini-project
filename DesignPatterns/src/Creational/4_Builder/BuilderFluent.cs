namespace Creational.Builder;

public record Order(
    string Customer,
    IReadOnlyList<string> Items,
    string? coupon,
    string Address,
    bool IsGiftWrap
);

public class OrderBuilder
{
    private string _customer = "";
    private readonly List<string> _items = new();
    private string? _coupon;
    private string _address = "";
    private bool _isGiftWrap;

    public OrderBuilder ForCustomer(string customer)
    {
        _customer = customer;
        return this;
    }

    public OrderBuilder AddItem(string item)
    {
        _items.Add(item);
        return this;
    }

    public OrderBuilder AddCoupon(string coupon)
    {
        _coupon = coupon;
        return this;
    }

    public OrderBuilder CreateAddress(string address)
    {
        _address = address;
        return this;
    }

    public OrderBuilder ApplyGiftWrap(bool isGiftWrap)
    {
        _isGiftWrap = isGiftWrap;
        return this;
    }

    public Order Build()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("An order needs at least one item.");
        }

        if (string.IsNullOrWhiteSpace(_address))
        {
            throw new InvalidOperationException("A shipping address is required.");
        }

        Order order = new Order(_customer, _items.ToList(), _coupon, _address, _isGiftWrap);

        return order;
    }
}
