using ProductCatalog.App.Models.Entities;
using ProductCatalog.App.Repositories.Interfaces;

namespace ProductCatalog.Infra.Repositories;

public class SkuPriceRepository : ISkuPriceRepository
{
    public Task<Money?> GetPriceAsync(string skuId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetPriceAsync(string skuId, Money amount, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
