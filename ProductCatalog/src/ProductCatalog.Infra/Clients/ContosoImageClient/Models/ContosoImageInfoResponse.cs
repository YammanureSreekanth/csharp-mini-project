using System.Text.Json.Serialization;

namespace ProductCatalog.Infra.Clients;

public sealed record ContosoImageInfoResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("author")] string Author,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("download_url")] string DownloadUrl
);
