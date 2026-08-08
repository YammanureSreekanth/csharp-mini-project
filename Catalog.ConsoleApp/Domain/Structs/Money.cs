using Domain.Enums;
namespace Domain.Structs
{
    public struct Money
    {
        public decimal Amount {get; set;}
        public Currency CurrencyValue {get; set;}

        public Money(decimal amount, Currency currencyValue)
        {
            Amount = amount;
            CurrencyValue = currencyValue;
        }
    }
}