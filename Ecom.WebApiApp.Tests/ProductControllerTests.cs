using Ecom.WebApiApp.DTOMapper;
using Ecom.WebApiApp.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Ecom.WebApiApp.Repos;
using Ecom.WebApiApp.Controllers;

namespace Ecom.WebApiApp.Tests;

public class ProductControllerTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {   

        _mockRepo.Setup(r => r.GetByIdAsync("1"))
                 .ReturnsAsync(new Product { 
                        Id = "1",
                        Name = "Test Product",
                        Description = "Testing",
                        IsOnline = true,
                        IsSearchable = false,
                        FabricCode = "12.1.2/0",
                        CreatedTime = DateTime.Now,
                        LastModifiedTime = DateTime.Now
                    }
                 );
                 
        ProductController? controller = new ProductController(_mockRepo.Object);

        ActionResult<ProductDto>? result = await controller.GetProduct("1");

        OkObjectResult? okResult = Assert.IsType<OkObjectResult>(result.Result);
        ProductDto? product = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal("1", product.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {

        _mockRepo.Setup(r => r.GetByIdAsync("1"))
                    .ReturnsAsync(new Product { 
                        Id = "1",
                        Name = "Test Product",
                        Description = "Testing",
                        IsOnline = true,
                        IsSearchable = false,
                        FabricCode = "12.1.2/0",
                        CreatedTime = DateTime.Now,
                        LastModifiedTime = DateTime.Now
                    });

        
        ProductController? controller = new ProductController(_mockRepo.Object);

        ActionResult<ProductDto>? result = await controller.GetProduct("999");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData("1", true)]   // existing product: found
    [InlineData("999", false)] // missing product: not found
    public async Task GetById_ReturnsExpectedResult(string id, bool shouldExist)
    {
        if (shouldExist)
        {

            _mockRepo.Setup(r => r.GetByIdAsync("1"))
                    .ReturnsAsync(new Product { 
                        Id = "1",
                        Name = "Test Product",
                        Description = "Testing",
                        IsOnline = true,
                        IsSearchable = false,
                        FabricCode = "12.1.2/0",
                        CreatedTime = DateTime.Now,
                        LastModifiedTime = DateTime.Now
                    });

        }

        ProductController? controller = new ProductController(_mockRepo.Object);

        ActionResult<ProductDto>? result = await controller.GetProduct(id);

        if (shouldExist)
        {
            Assert.IsType<OkObjectResult>(result.Result);   
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}