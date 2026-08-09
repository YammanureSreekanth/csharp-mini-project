using System.Text.Json;
using Domain.Classes.Product;
using Domain.Interfaces;
using Factories;
using Microsoft.Data.SqlClient;

namespace DataAccess {
    public class ProductRepository : IProductRepository
    {
        public List<BaseProduct> GetByCategory(string Id)
        {
            throw new NotImplementedException();
        }

        public BaseProduct? GetById(string Id)
        {
            const string GET_PRODUCT_BY_ID = "SELECT * FROM dbo.Products WHERE Id = @Id";
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "Id", Id }
            };
            List<BaseProduct> baseProducts = MySQLConnection.RunQuery<BaseProduct>(GET_PRODUCT_BY_ID ,keyValuePairs, SqlDataReaderProcessor);
            if (baseProducts.Count == 0)
            {
                return null;
            }
            return baseProducts.First();
        }

        public List<VariationProduct> VariationsByMasterProduct(string masterProductId)
        {
            throw new NotImplementedException();
        }

        public BaseProduct? SqlDataReaderProcessor(SqlDataReader reader)
        {
            Dictionary<string, object> rowData = new Dictionary<string, object>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                // Console.WriteLine($"Prop Name {reader.GetName(i)} \t {reader[i]}");
                rowData.Add(reader.GetName(i), reader[i]);
            }

            string Id = (string)rowData["Id"];
            string Name = (string)rowData["Name"];
            byte type = (byte)rowData["Type"];
            BaseProduct product = ProductFactory.Create(Id, Name, type, rowData);
            if (product is ISellable sellable)
            {
                Console.WriteLine(sellable.Price.Amount);
            }

            string productStr = JsonSerializer.Serialize(product);
            Console.WriteLine(productStr);
            return product;
        }
    }
}