using System.Text.Json;
using Catalog.ConsoleApp.CustomAttributes;
using Catalog.ConsoleApp.Domain.Classes.Category;
using Catalog.ConsoleApp.Domain.Classes.Product;
using Catalog.ConsoleApp.Domain.Interfaces;
using Catalog.ConsoleApp.Domain.Structs;
using Catalog.ConsoleApp.Factories;
using Microsoft.Data.SqlClient;

namespace Catalog.ConsoleApp.DataAccess {
    [Info("Sreekanth", "1.0.0")]
    public class ProductRepository : IProductRepository, IDisposable
    {
        private readonly MySQLConnection _db;

        public ProductRepository(string connectionString) => _db = new MySQLConnection(connectionString);

        public List<BaseProduct> GetByCategory(string Id)
        {
            throw new NotImplementedException();
        }

        public BaseProduct? GetById(string Id)
        {
            const string GET_PRODUCT_BY_ID_QUERY = "SELECT * FROM dbo.Products WHERE Id = @Id";
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "Id", Id }
            };
            List<BaseProduct> baseProducts = _db.RunQuery<BaseProduct>(GET_PRODUCT_BY_ID_QUERY ,keyValuePairs, SqlDataReaderProcessor);
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
            BaseProduct product = ProductFactory.Create(Id, Name, type, rowData, reader);
            if (product is ISellable sellable)
            {
                Console.WriteLine(sellable.Price.Amount);
            }

            ProductImage productImage = new ProductImage();
            productImage.Alt = "Black Tailored Fit Lazio Dinner Jacket";
            productImage.Title = "Black Tailored Fit Lazio Dinner Jacket";
            productImage.Path = "products/Jackets/default/Winter/C1199_1";
            product.AddImages(productImage);

            ProductCategoryAssignment productCategory = new ProductCategoryAssignment();
            productCategory.Category = new Category("suits", "Suits", "");
            productCategory.IsPrimary = true;
            product.AssignCateggory(productCategory);

            string productStr = JsonSerializer.Serialize(product);
            Console.WriteLine(productStr);
            return product;
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }

}