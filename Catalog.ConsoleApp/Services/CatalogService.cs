using DataAccess;
using Domain.Classes.Product;

namespace Services
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