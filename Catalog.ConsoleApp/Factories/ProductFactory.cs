using Domain.Classes.Product;
using Domain.Enums;
using Domain.Structs;

namespace Factories
{
    public static class ProductFactory
    {
        public static BaseProduct Create(string id, string name, int type, Dictionary<string, object> rowData)
        {
            BaseProduct product = null;
            if (type == 2)
            {
                decimal PriceAmount = (decimal)rowData["PriceAmount"];
                // string PriceCurrency = (string)rowData["PriceCurrency"];
                Money productPrice = new Money(PriceAmount, Currency.EUR);

                int StockAts = (int)rowData["StockAts"];
                int StockPreorder = (int)rowData["StockPreorder"];
                // DateOnly StockInstockDate = (DateOnly)rowData["StockInstockDate"];
                bool StockIsPerpetual = (bool)rowData["StockIsPerpetual"];
                StockStatus stockStatus = new StockStatus(StockAts, StockPreorder, StockIsPerpetual);
                product = new StandardProduct(id, name, productPrice, stockStatus)
                {
                    IsOnline = (bool)rowData["IsOnline"],
                    IsSearchable = (bool)rowData["IsSearchable"]
                };

                string SeoPageTitle = (string)rowData["SeoPageTitle"];
                object rawSeoPageKeywords = rowData["SeoPageKeywords"];
                string? SeoPageKeywords = rawSeoPageKeywords is DBNull ? null : (string)rawSeoPageKeywords;
                SeoInfo SEO = new SeoInfo(SeoPageTitle);
                if (SeoPageKeywords is not null)
                {
                    SEO.PageKeywords = SeoPageKeywords;
                }
                product.SEO = SEO;

                product.Type = ProductType.STANDARD_PRODUCT;
            }
            return product;
        }
    }
}