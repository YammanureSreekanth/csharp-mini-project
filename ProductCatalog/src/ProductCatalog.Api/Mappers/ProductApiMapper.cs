using ProductCatalog.Api.Models.Requests.Product;
using ProductCatalog.Api.Models.Responses.Product;
using ProductCatalog.App.Models;
using ProductCatalog.App.Models.Commands;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Api.Mappers;

public static class ProductApiMapper
{
    public static CreateProductCommand ToCommand(CreateProductRequest request) =>
        new(
            request.Id,
            request.Name,
            request.Description,
            request.CategoryId,
            request.Searchable,
            request.Online,
            request.Price,
            request.Ats
        );

    public static UpdateProductCommand ToCommand(UpdateProductRequest request) =>
        new(
            request.Id,
            request.Name,
            request.Description,
            request.CategoryId,
            request.Searchable,
            request.Online,
            request.Price,
            request.Ats
        );

    public static ProductResponse ToResponse(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.CategoryId,
            product.Searchable,
            product.Online,
            product.Price,
            product.Ats,
            Image: null
        );

    public static ProductResponse ToResponse(ProductWithImage details) =>
        new(
            details.Product.Id,
            details.Product.Name,
            details.Product.Description,
            details.Product.CategoryId,
            details.Product.Searchable,
            details.Product.Online,
            details.Product.Price,
            details.Product.Ats,
            Image: details.Image is null
                ? null
                : new ProductImageResponse(
                    details.Image.Url,
                    details.Image.Width,
                    details.Image.Height,
                    details.Image.Author
                )
        );
}
