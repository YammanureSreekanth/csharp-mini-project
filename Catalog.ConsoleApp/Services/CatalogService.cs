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
            Category? rootCategory = CatalogMenu(categories);
            foreach (Category c in rootCategory.Childs())
            {
                Console.WriteLine(c.Name);
            }
            return [];
        }

        public List<string> GetProductsByCategoryId(string categoryId)
        {
            Logger.Debug("Calling Method {0} from Service Class is {1}", "GetProductsByCategoryId", "CatalogService");
            List<string> productIds = _categoryRepo.GetProductAssigegmentsByCategoryId(categoryId);
            foreach (string productId in productIds)
            {
                Console.WriteLine($"{productId}");
            }
            return productIds;
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

        public static Category? CatalogMenu(List<Category> rows)
        {
            Dictionary<string, Category>? byId = rows.ToDictionary(r => r.Id, r => r);

            Category? root = null;

            foreach (Category row in rows)
            {
                Category category = byId[row.Id];

                if (row.ParentCategoryId is null)
                {
                    root = category;
                }
                else
                {
                    byId[row.ParentCategoryId].AddSubCategory(category);
                }
            }

            return root;
        }
    }
}