using Domain.Classes.Product;
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

        public BaseProduct SqlDataReaderProcessor(SqlDataReader sqlDataReader)
        {
            return sqlDataReader[];
        }
    }
}