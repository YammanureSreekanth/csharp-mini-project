using Domain.Classes.Category;

namespace DataAccess
{
    public interface ICategoryRepository
    {
        public List<Category> GetAll();
        public Category GetById();
        public List<Category> SubCategories();
    }
}