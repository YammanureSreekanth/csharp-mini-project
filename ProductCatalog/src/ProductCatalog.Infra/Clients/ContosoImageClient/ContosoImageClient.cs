using System.Net.Http.Json;
using ProductCatalog.App.Clients.Interfaces;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Infra.Clients;

public class ContosoImageClient(HttpClient httpClient) : IProductImageClient
{
    public async Task<ProductImageInfo> GetImageAsync(string productId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetFromJsonAsync<ContosoImageInfoResponse>(
            "/id/237/info",
            cancellationToken);

        return new ProductImageInfo(response!.Url, response.Width, response.Height, response.Author);
    }
}