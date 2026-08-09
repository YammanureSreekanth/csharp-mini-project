
using Catalog.ConsoleApp.CustomAttributes;
using Catalog.ConsoleApp.DataAccess;
using Catalog.ConsoleApp.Services;

namespace Catalog.ConsoleApp
{
    public class Program
    {
        public static void Main()
        {
            ICategoryRepository categoryRepository = new CategoryRepository();
            IProductRepository productRepository = new ProductRepository();
            Type type = typeof (ProductRepository);
            object[] attrs = type.GetCustomAttributes(typeof(InfoAttribute), false);
            foreach (InfoAttribute attr in attrs)
            {
                Console.WriteLine($"Author {attr.Author} and Version {attr.Version}");
            }
            CatalogService catalogService = new CatalogService(categoryRepository, productRepository);
            catalogService.GetProductById("D005");
        }
    }
}