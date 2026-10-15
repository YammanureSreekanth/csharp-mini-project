namespace ProductCatalog.Api.Models.Responses.Product;

public sealed record ProductImageResponse(string Url, int Width, int Height, string? Author);
