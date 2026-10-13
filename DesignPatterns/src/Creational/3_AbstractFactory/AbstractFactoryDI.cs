using Microsoft.Extensions.DependencyInjection;

namespace Creational.AbstractFactory;

public class RegionalCheckout
{
    private readonly IRegionFactory _region;

    public RegionalCheckout(IRegionFactory region)
    {
        _region = region;
    }

    public string Run(decimal subtotal) =>
        _region.PaymentGateway().Pay(subtotal + _region.TaxCalculator().TaxFor(subtotal));
}
