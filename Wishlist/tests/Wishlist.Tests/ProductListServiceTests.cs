using Moq;
using Wishlist.Core.Contracts;
using Wishlist.Core.DTOs;
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
        IReadOnlyList<ProductListDto>? productLists = await _service.GetListsForCustomerAsync(customerOne, CancellationToken.None);

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
        ProductListDto productListDto = await _service.GetListAsync(listOneId, CancellationToken.None);

        //Assert
        Assert.Equal("FeestDag", productListDto.Name);

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
        ProductListDto listDto = await _service.GetListAsync(listId, CancellationToken.None);

        //Assert
        Assert.Equal(type, listDto.Type);
    }

    [Theory]
    [InlineData("b183381b-2009-4301-89b1-2c5c11770787", true)]
    [InlineData("b183381b-2009-4301-89b1-2c5c11770788", false)]
    public async Task Set_ProductList_Visibility(Guid productListId, bool isPublic)
    {

        //Arrange
        string customerOne = "CustomerOne";

        ProductList productList = new ProductList
        {
            Id = productListId,
            Name = "Wedding",
            CustomerId = customerOne
        };

        productList.Type = ProductListType.TYPE_WISH_LIST;
        productList.IsPublic = isPublic;

        _repo.Setup(repo => repo.GetByIdAsync(productListId, CancellationToken.None))
            .ReturnsAsync(productList);

        _repo.Setup(r => r.UpdateListAsync(productList,CancellationToken.None));

        _repo.Setup(r => r.SaveChangesAsync(CancellationToken.None));
        
        //Act
        ProductListDto? listDto = await _service.UpdateListAsync(productListId, new UpdateListVisibilityRequest(isPublic), CancellationToken.None);

        //Assert
        Assert.Equal(isPublic, listDto.IsPublic);
    }

    public static IEnumerable<object[]> ItemRequests =[
            new object[] { "b183381b-2009-4301-89b1-2c5c11770788", new CreateProductListItemRequest("SN12323", true, 1) },
            new object[] { "c2f4a1de-77aa-4c9b-9d10-1a2b3c4d5e6f", new CreateProductListItemRequest("SN99999", false, 3) },
    ];

    [Theory]
    [MemberData(nameof(ItemRequests))]
    public async Task AddProduct_To_List(Guid listId, CreateProductListItemRequest req)
    {
        //Arrange
        string customerId = "Customer-12";
        ProductList productList = new ProductList
        {
            Id = listId,
            Name = "Wedding",
            CustomerId = customerId
        };

        Guid itemId = Guid.NewGuid();
        ProductListItem productListItem = new ProductListItem
        {
            Id = itemId,
            ProductId = req.ProductId,
            ProductListId = listId,
            IsPublic = req.IsPublic,
            Quantity = req.Quantity
        };

        _repo.Setup(repo => repo.GetByIdAsync(listId, CancellationToken.None))
            .ReturnsAsync(productList);

        _repo.Setup(repo => repo.ExistProductIdByListId(listId, req.ProductId, CancellationToken.None))
            .ReturnsAsync(value: false);
        
        _repo.Setup(repo => repo.AddProductAsync(productListItem, CancellationToken.None));

        _repo.Setup(r => r.SaveChangesAsync(CancellationToken.None));

        //Act
        ProductListItemDto listItemDto = await _service.AddProductAsync(listId, req, CancellationToken.None);

        //Assert
        Assert.Equal(req.ProductId, listItemDto.ProductId);
    }
}