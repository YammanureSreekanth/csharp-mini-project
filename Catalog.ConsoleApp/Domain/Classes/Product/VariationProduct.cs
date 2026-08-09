using Catalog.ConsoleApp.Domain.Enums;
using Catalog.ConsoleApp.Domain.Interfaces;
using Catalog.ConsoleApp.Domain.Structs;

namespace Catalog.ConsoleApp.Domain.Classes.Product
{
    public class VariationProduct : BaseProduct, ISellable
    {
        public MasterProduct Master {get; set;}
        public Money Price { get; private set; }
        public StockStatus Availability { get; private set; }
        public VariationProduct(string id, string name, MasterProduct master, Money price, StockStatus availability)
        : base(id, name,ProductType.VARIATION_PRODUCT)
        {
            Master = master;
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