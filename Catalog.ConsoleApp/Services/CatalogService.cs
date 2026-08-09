using Catalog.ConsoleApp.DataAccess;
using Catalog.ConsoleApp.Domain.Classes.Product;

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
        public BaseProduct? GetProductById(string Id)
        {
            BaseProduct? baseProduct = _productRepo.GetById(Id);
            if (baseProduct is null)
            {
                return null;
            }
            return baseProduct;
        }
    }
}