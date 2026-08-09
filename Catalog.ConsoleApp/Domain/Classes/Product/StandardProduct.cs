using Catalog.ConsoleApp.Domain.Enums;
using Catalog.ConsoleApp.Domain.Interfaces;
using Catalog.ConsoleApp.Domain.Structs;

namespace Catalog.ConsoleApp.Domain.Classes.Product
{
    public class StandardProduct : BaseProduct, ISellable
    {
        public Money Price { get; set; }
        public StockStatus Availability { get; set; }
        public StandardProduct(string id, string name, Money price, StockStatus availability)
        : base(id, name,ProductType.STANDARD_PRODUCT)
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