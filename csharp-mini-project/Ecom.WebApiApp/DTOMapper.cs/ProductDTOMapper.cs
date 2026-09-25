using Ecom.WebApiApp.DTOMapper;

namespace Ecom.WebApiApp.Models.DTOMapper;

/// <summary>
/// ProductDtoMapper
/// </summary>
public static class ProductDtoMapper
{
    /// <summary>
    /// This converts the model to dto
    /// </summary>
    /// <param name="product">Product Model</param>
    /// <returns>Returns ProductDto</returns>
    public static ProductDto MapToDTO(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            IsOnline = product.IsOnline,
            IsSearchable = product.IsSearchable,
            FabricCode = product.FabricCode,
            Price = product.Price
        };
    }

    public static Product MapDtoToModel(ProductDto product)
    {
        return new Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            IsOnline = product.IsOnline,
            IsSearchable = product.IsSearchable,
            FabricCode = product.FabricCode,
            Price = product.Price,
            LastModifiedTime = new DateTime()
        };
    }
}