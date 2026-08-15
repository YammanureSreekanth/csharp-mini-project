using Catalog.ConsoleApp.CustomExtensions;
using Catalog.ConsoleApp.Domain.Classes.Product;
using Catalog.ConsoleApp.Domain.Enums;
using Catalog.ConsoleApp.Domain.Structs;
using Microsoft.Data.SqlClient;

namespace Catalog.ConsoleApp.Factories
{
    public static class ProductFactory
    {
        public static BaseProduct Create(string id, string name, int type, Dictionary<string, object> rowData, SqlDataReader sqlDataReader)
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
                bool isdbNull = SqlDataReaderExtensions.isValueNullInDB(sqlDataReader,"SeoPageKeywords");
                string? SeoPageKeywords = isdbNull ? null : (string)rawSeoPageKeywords;
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