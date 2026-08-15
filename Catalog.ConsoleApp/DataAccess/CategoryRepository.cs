using Catalog.ConsoleApp.Domain.Classes.Category;
using Catalog.ConsoleApp.Factories;
using Microsoft.Data.SqlClient;

namespace Catalog.ConsoleApp.DataAccess {
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetAll()
        {
            const string GET_CATEGORY_QUERY = "SELECT * FROM dbo.Categories";
            List<Category> categories = MySQLConnection.RunNonQuery<Category>(GET_CATEGORY_QUERY, SqlDataReaderProcessor);
            return categories;
        }

        public Category GetById()
        {
            throw new NotImplementedException();
        }

        public List<Category> SubCategories()
        {
            throw new NotImplementedException();
        }

         public Category SqlDataReaderProcessor(SqlDataReader reader)
        {
            Dictionary<string, object> rowData = new Dictionary<string, object>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                // Console.WriteLine($"Prop Name {reader.GetName(i)} \t {reader[i]}");
                rowData.Add(reader.GetName(i), reader[i]);
            }

            Category category = CategoryFactory.GetCategory(rowData);

            return category;
        }
    }
}