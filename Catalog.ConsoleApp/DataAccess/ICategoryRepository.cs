using Catalog.ConsoleApp.Domain.Classes.Category;

namespace  Catalog.ConsoleApp.DataAccess
{
    public interface ICategoryRepository
    {
        public List<Category> GetAll();
        public Category GetById();
        public List<Category> SubCategories();
    }
}