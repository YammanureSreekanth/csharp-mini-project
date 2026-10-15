using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.App.Models;

public sealed record ProductWithImage(Product Product, ProductImageInfo? Image);
