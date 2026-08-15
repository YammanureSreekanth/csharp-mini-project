using Catalog.ConsoleApp.Domain.Classes.Category;
using Catalog.ConsoleApp.Domain.Classes.Product;

namespace  Catalog.ConsoleApp.DataAccess
{
    public interface ICategoryRepository
    {
        public List<Category> GetAll();
        public List<string> GetProductAssigegmentsByCategoryId(string categoryId);
        public Category GetById();
        public List<Category> SubCategories();
    }
}