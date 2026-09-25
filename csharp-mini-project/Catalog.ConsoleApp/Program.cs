
using Catalog.ConsoleApp.CustomAttributes;
using Catalog.ConsoleApp.DataAccess;
using Catalog.ConsoleApp.Services;
using Microsoft.Extensions.Logging;

namespace Catalog.ConsoleApp
{
    public class Program
    {
        public static async Task Main()
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder =>
            {
                builder
                .AddSimpleConsole(options => options.IncludeScopes = true)
                .SetMinimumLevel(LogLevel.Debug);
            });
            
            ILogger logger = factory.CreateLogger<Program>();

            logger.LogInformation("Hello World! Logging is {Description}.", "fun");

            string DbCon = "Data Source=localhost;Initial Catalog=CatalogDb;User ID=sa;Password=PraticeApp@2031;Pooling=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Authentication=SqlPassword;Application Name=vscode-mssql;Application Intent=ReadWrite;Command Timeout=30";

            ILogger<CategoryRepository> categoryRepoLogger = factory.CreateLogger<CategoryRepository>();

            ILogger<ProductRepository> productRepoLogger = factory.CreateLogger<ProductRepository>();

            ILogger<MySQLConnection> dbLogger = factory.CreateLogger<MySQLConnection>();

            ICategoryRepository categoryRepository = new CategoryRepository(DbCon, categoryRepoLogger, dbLogger);
            
            IProductRepository productRepository = new ProductRepository(DbCon, productRepoLogger, dbLogger);
            
            Type type = typeof (ProductRepository);
            
            object[] attrs = type.GetCustomAttributes(typeof(InfoAttribute), false);
            
            foreach (InfoAttribute attr in attrs)
            {
                logger.LogInformation("Author {Name} and Version {Version}", attr.Author, attr.Version);
            }
            
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken cancellationToken = cancellationTokenSource.Token;

            cancellationTokenSource.CancelAfter(1000);

            CatalogService catalogService = new CatalogService(categoryRepository, productRepository);
            
            // catalogService.GetProductById("D005");
            
            await catalogService.Catalog(cancellationToken);

            // catalogService.GetProductsByCategoryId("black-tie-collection");
        }
    }
}