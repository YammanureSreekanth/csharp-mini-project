using Catalog.ConsoleApp.Domain.Classes.Product;

namespace Catalog.ConsoleApp.DataAccess
{
    public interface IProductRepository
    {
        public List<BaseProduct> GetByCategory(string Id);
        public BaseProduct? GetById(string Id);
        public List<VariationProduct> VariationsByMasterProduct(string masterProductId);
    }
}