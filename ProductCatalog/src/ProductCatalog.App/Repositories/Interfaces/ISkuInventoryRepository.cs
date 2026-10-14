namespace ProductCatalog.App.Repositories.Interfaces;

public interface ISkuInventoryRepository
{
    Task SetInventoryAsync(string skuId, int ats, CancellationToken cancellationToken);
    Task<int?> GetInventoryAsync(string skuId, CancellationToken cancellationToken);
}
