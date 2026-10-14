using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.App.Repositories.Interfaces;

public interface ISkuPriceRepository
{
    Task SetPriceAsync(string skuId, Money amount, CancellationToken cancellationToken);
    Task<Money?> GetPriceAsync(string skuId, CancellationToken cancellationToken);
}
