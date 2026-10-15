using System.Text.Json.Serialization;

namespace ProductCatalog.Infra.Clients;

public sealed record FabrikamImageInfoResponse(
    [property: JsonPropertyName("albumId")] int AlbumId,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("thumbnailUrl")] string ThumbnailUrl
);
