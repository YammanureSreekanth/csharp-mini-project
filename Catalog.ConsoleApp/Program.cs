
using DataAccess;
using Services;

namespace Catalog.ConsoleApp
{
    public class Program
    {
        public static void Main()
        {
            ICategoryRepository categoryRepository = new CategoryRepository();
            IProductRepository productRepository = new ProductRepository();
            CatalogService catalogService = new CatalogService(categoryRepository, productRepository);
            catalogService.GetProductById("D005");
        }
    }
}