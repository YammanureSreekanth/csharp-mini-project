using Moq;
using Wishlist.Core.Entities;
using Wishlist.Core.Enums;
using Wishlist.Core.Interfaces;
using Wishlist.Functions.Services;

namespace Wishlist.Tests;

public class ProductListServiceTests
{
    private readonly Mock<IProductListRepository> _repo = new();
    private readonly ProductListService _service;

    public ProductListServiceTests()
    {
        _service = new ProductListService(_repo.Object);
    }

    [Fact]
    public async Task GetList_ByCustomerId()
    {
        // Arrage
        Guid listOneId = Guid.NewGuid();

        string customerOne = "CustomerOne";

        ProductList productListOne = new ProductList
        {
            Id = listOneId,
            Name = "Birthday",
            CustomerId = customerOne
        };

        productListOne.Type = ProductListType.TYPE_WISH_LIST;
        productListOne.IsPublic = true;

        Guid listTwoId = Guid.NewGuid();

        ProductList productListTwo = new ProductList
        {
            Id = listTwoId,
            Name = "Anniversary",
            CustomerId = customerOne
        };

        productListTwo.Type = ProductListType.TYPE_WISH_LIST;
        productListTwo.IsPublic = true;

        List<ProductList> list = new List<ProductList>();
        list.Add(productListOne);
        list.Add(productListTwo);

        _repo.Setup(repo => repo.GetByCustomerAsync(customerOne, CancellationToken.None))
                .ReturnsAsync((IReadOnlyList<ProductList>) list);
            
        // Act
        IReadOnlyList<ProductList>? productLists = await _service.GetListsForCustomerAsync(customerOne, CancellationToken.None);

        //Assert
        Assert.Equal(2, productLists.Count);
    }

    [Fact]
    public async Task AddProductList_To_Customer()
    {
        //Arrage
        Guid listOneId = Guid.NewGuid();

        string customerOne = "CustomerOne";

        ProductList productListOne = new ProductList
        {
            Id = listOneId,
            Name = "FeestDag",
            CustomerId = customerOne
        };

        productListOne.Type = ProductListType.TYPE_WISH_LIST;
        productListOne.IsPublic = true;

        _repo.Setup(repo => repo.GetByIdAsync(listOneId, CancellationToken.None))
                .ReturnsAsync(productListOne);

        //Act
        ProductList productList = await _service.GetListAsync(listOneId, CancellationToken.None);

        //Assert
        Assert.Equal("FeestDag", productList.Name);

    }

    [Theory]
    [InlineData("01f1b7a3-7e18-45e4-b7be-05bd08651844", ProductListType.TYPE_WISH_LIST)]
    [InlineData("b183381b-2009-4301-89b1-2c5c11770787", ProductListType.TYPE_SHOPPING_LIST)]
    public async Task GetList_ById(Guid listId, ProductListType type)
    {
        //Arrange
        string customerOne = "CustomerOne";

        ProductList productListOne = new ProductList
        {
            Id = listId,
            Name = "FeestDag",
            CustomerId = customerOne
        };

        productListOne.Type = type;
        productListOne.IsPublic = true;

        _repo.Setup(repo => repo.GetByIdAsync(listId, CancellationToken.None))
            .ReturnsAsync(productListOne);
        
        //Act
        ProductList list = await _service.GetListAsync(listId, CancellationToken.None);

        //Assert
        Assert.Equal(type, list.Type);
    }
}