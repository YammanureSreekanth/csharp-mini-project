using Catalog.ConsoleApp.DataAccess;
using Catalog.ConsoleApp.Domain.Classes.Category;
using Catalog.ConsoleApp.Domain.Classes.Product;
using Catalog.ConsoleApp.Logging;

namespace Catalog.ConsoleApp.Services
{
    public class CatalogService
    {
        public ICategoryRepository _categoryRepo;
        public IProductRepository _productRepo;
        public CatalogService(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepo = categoryRepository;
            _productRepo = productRepository;
        }

        public List<Category> Catalog()
        {
            Logger.Debug("Calling Method {0} from Service Class is {1}", "Catalog", "CatalogService");
            List<Category> categories = _categoryRepo.GetAll();
            Console.WriteLine("Welcome to Suitsupply");
            foreach (Category category in categories)
            {
                Console.WriteLine($"{category.Name}");
            }
            return categories;
        }

        public BaseProduct? GetProductById(string Id)
        {
            Logger.Debug("Calling Method {0} from Service Class is {1}", "GetProductById", "CatalogService");
            BaseProduct? baseProduct = _productRepo.GetById(Id);
            if (baseProduct is null)
            {
                return null;
            }
            return baseProduct;
        }
    }
}