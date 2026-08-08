using Domain.Enums;
using Domain.Interfaces;
using Domain.Structs;

namespace Domain.Classes.Product
{
    public class VariationProduct : BaseProduct, ISellable
    {
        public Money Price { get; private set; }
        public StockStatus Availability { get; private set; }
        public VariationProduct(string id, string name, Money price, StockStatus availability)
        : base(id, name,ProductType.STANDARD_PRODUCT )
        {
            Price = price;
            Availability = availability;
        }
        public override string GetPriceDisplay()
        {
            throw new NotImplementedException();
        }

        public override bool MatchesKeyword(string keyword)
        {
            throw new NotImplementedException();
        }
    }
}