using Catalog.ConsoleApp.Domain.Classes.Category;
using Catalog.ConsoleApp.Factories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Catalog.ConsoleApp.DataAccess {
    public class CategoryRepository : ICategoryRepository, IDisposable
    {
        private readonly MySQLAsyncConnection _db;
        private readonly ILogger<CategoryRepository> _logger;
        private static readonly EventId CategoryLoad = new(1001, nameof(CategoryLoad));

        public CategoryRepository(string connectionString, ILogger<CategoryRepository> logger, ILogger<MySQLConnection> dbLogger)
        {
            _db = new MySQLAsyncConnection(connectionString, dbLogger);
            _logger = logger; 
        }

        public async Task<IReadOnlyList<Category>> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Loading all categories");

            const string GET_CATEGORY_QUERY = "SELECT * FROM dbo.Categories";

            List<Category> categories = await _db.RunNonQueryAsync<Category>(GET_CATEGORY_QUERY, SqlDataReaderProcessor, cancellationToken);

            _logger.LogInformation(CategoryLoad, "Query Successfull: {Query}", GET_CATEGORY_QUERY);
            
            return categories;
        }

        public async Task<IReadOnlyList<string>> GetProductAssigegmentsByCategoryId(string categoryId, CancellationToken cancellationToken)
        {
            
            using (_logger.BeginScope("GetProductAssigegmentsByCategoryId for {categoryId} & {RequestId}", categoryId, Guid.NewGuid()))
            {
                _logger.LogInformation("Loading all Products Assigned");
    
                const string GET_PRODUCTS_ID_QUERY = "SELECT * FROM dbo.ProductCategoryAssignments where CategoryId = @CategoryId";

                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
                {
                    { "CategoryId", categoryId }
                };
        
                Func<SqlDataReader, string> DataReader = (SqlDataReader reader) =>
                {
                    return reader["ProductId"]?.ToString();
                };

                List<string> productIds = await _db.RunQueryAsync<string>(GET_PRODUCTS_ID_QUERY,keyValuePairs, DataReader, cancellationToken);

                 _logger.LogInformation("Loaded {Count} Products", productIds.Count);

                return productIds;                
            }
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
            string id = reader.GetString(reader.GetOrdinal("Id"));

            string name = reader.GetString(reader.GetOrdinal("Name"));

            string? parentId = reader.IsDBNull(reader.GetOrdinal("ParentCategoryId"))
                ? null
                : reader.GetString(reader.GetOrdinal("ParentCategoryId"));


            return new Category(id, name, parentId);
        }

        public void Dispose() => _db.Dispose();

        public Task<Category> GetById(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<Category>> SubCategories(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}