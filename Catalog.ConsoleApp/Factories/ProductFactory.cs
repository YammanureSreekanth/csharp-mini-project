using Domain.Classes.Product;
using Domain.Structs;

namespace Factory
{
    public static class ProductFactory
    {
        public static BaseProduct Create(string id, string name, Money price, StockStatus availability)
        {
            StandardProduct standardProduct = new StandardProduct(id, name, price, availability);
            return standardProduct;
        }
    }
}