
using Catalog.ConsoleApp.CustomAttributes;
using Catalog.ConsoleApp.DataAccess;
using Catalog.ConsoleApp.Services;

namespace Catalog.ConsoleApp
{
    public class Program
    {
        public static void Main()
        {
            string DbCon = "Data Source=localhost;Initial Catalog=CatalogDb;User ID=sa;Password=PraticeApp@2031;Pooling=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Authentication=SqlPassword;Application Name=vscode-mssql;Application Intent=ReadWrite;Command Timeout=30";

            ICategoryRepository categoryRepository = new CategoryRepository(DbCon);
            IProductRepository productRepository = new ProductRepository(DbCon);
            Type type = typeof (ProductRepository);
            object[] attrs = type.GetCustomAttributes(typeof(InfoAttribute), false);
            foreach (InfoAttribute attr in attrs)
            {
                Console.WriteLine($"Author {attr.Author} and Version {attr.Version}");
            }
            CatalogService catalogService = new CatalogService(categoryRepository, productRepository);
            // catalogService.GetProductById("D005");
            catalogService.Catalog();
            // catalogService.GetProductsByCategoryId("black-tie-collection");
        }
    }
}