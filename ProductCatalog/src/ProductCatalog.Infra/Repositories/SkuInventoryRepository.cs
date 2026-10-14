using ProductCatalog.App.Repositories.Interfaces;

namespace ProductCatalog.Infra.Repositories;

public class SkuInventoryRepository : ISkuInventoryRepository
{
    public Task<int?> GetInventoryAsync(string skuId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetInventoryAsync(string skuId, int ats, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
