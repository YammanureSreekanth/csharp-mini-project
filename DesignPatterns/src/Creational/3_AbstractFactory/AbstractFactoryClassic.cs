namespace Creational.AbstractFactory;

public class Checkout
{
    private readonly IRegionFactory _region;

    public Checkout(IRegionFactory region)
    {
        _region = region;
    }

    public string Run(decimal subtotal)
    {
        decimal tax = _region.TaxCalculator().TaxFor(subtotal);
        return _region.PaymentGateway().Pay(subtotal + tax);
    }
}
