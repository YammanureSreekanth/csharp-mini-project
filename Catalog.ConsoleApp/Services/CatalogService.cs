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

        public async Task Catalog(CancellationToken cancellationToken)
        {
            Logger.Debug("Calling Method {0} from Service Class is {1}", "Catalog", "CatalogService");

            IReadOnlyList<Category> categories = await _categoryRepo.GetAll(cancellationToken);

            Console.WriteLine("Welcome to Suitsupply");

            Category? rootCategory = CatalogMenu(categories);

            foreach (Category c in rootCategory.Childs())
            {
                Console.WriteLine(c.Name);
            }
        }

        public async Task<IReadOnlyList<string>> GetProductsByCategoryId(string categoryId, CancellationToken cancellationToken)
        {
            Logger.Debug("Calling Method {0} from Service Class is {1}", "GetProductsByCategoryId", "CatalogService");

            IReadOnlyList<string> productIds = await _categoryRepo.GetProductAssigegmentsByCategoryId(categoryId, cancellationToken);

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

        public static Category? CatalogMenu(IReadOnlyList<Category> rows)
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