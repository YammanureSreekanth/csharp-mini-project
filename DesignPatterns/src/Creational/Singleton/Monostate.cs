namespace Creational.Singleton;

public class Monostate
{
    private static string _currency = "EUR";
    private static decimal _shopPrice = 199.5m;

    public string Currency
    {
        get => _currency;
        set => _currency = value;
    }

    public decimal ShopPrice
    {
        get => _shopPrice;
        set => _shopPrice = value;
    }
}
