using System.Net.Http.Json;
using ProductCatalog.App.Clients.Interfaces;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Infra.Clients;

public class FabrikamImageClient(HttpClient httpClient) : IProductImageClient
{
    public async Task<ProductImageInfo> GetImageAsync(string productId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetFromJsonAsync<FabrikamImageInfoResponse>(
            "/photos/1",
            cancellationToken);

        return new ProductImageInfo(
            response!.Url,
            Width: 600,
            Height: 600,
            Author: response.Title);
    }

}