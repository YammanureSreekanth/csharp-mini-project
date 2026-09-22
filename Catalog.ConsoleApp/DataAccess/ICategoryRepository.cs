using Catalog.ConsoleApp.Domain.Classes.Category;
using Catalog.ConsoleApp.Domain.Classes.Product;

namespace  Catalog.ConsoleApp.DataAccess
{
    public interface ICategoryRepository
    {
        public Task<IReadOnlyList<Category>> GetAll(CancellationToken cancellationToken);
        public Task<IReadOnlyList<string>> GetProductAssigegmentsByCategoryId(string categoryId, CancellationToken cancellationToken);
        public Task<Category> GetById(CancellationToken cancellationToken);
        public Task<List<Category>> SubCategories(CancellationToken cancellationToken);

    }
}