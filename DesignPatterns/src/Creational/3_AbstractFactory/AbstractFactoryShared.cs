namespace Creational.AbstractFactory;

public interface IPaymentGateway
{
    string Pay(decimal amount);
}

public interface ITaxCalculator
{
    decimal TaxFor(decimal amount);
}

public class IDealGateway : IPaymentGateway
{
    public string Pay(decimal amount) => $"[iDEAL] Gateway processing amount {amount}";
}

public class CardGateway : IPaymentGateway
{
    public string Pay(decimal amount) => $"[Card] Gateway processing amount {amount}";
}

public class EUTaxCalculator : ITaxCalculator
{
    public decimal TaxFor(decimal amount) => amount * 0.21m;
}

public class USTaxCalculator : ITaxCalculator
{
    public decimal TaxFor(decimal amount) => amount * 0.07m;
}

public interface IRegionFactory
{
    IPaymentGateway PaymentGateway();
    ITaxCalculator TaxCalculator();
}

public class EUFactory : IRegionFactory
{
    public IPaymentGateway PaymentGateway() => new IDealGateway();

    public ITaxCalculator TaxCalculator() => new EUTaxCalculator();
}

public class USFactory : IRegionFactory
{
    public IPaymentGateway PaymentGateway() => new CardGateway();

    public ITaxCalculator TaxCalculator() => new USTaxCalculator();
}
