using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.App.Clients.Interfaces;

public interface IProductImageClient
{
    Task<ProductImageInfo> GetImageAsync(string productId, CancellationToken cancellationToken);
}


