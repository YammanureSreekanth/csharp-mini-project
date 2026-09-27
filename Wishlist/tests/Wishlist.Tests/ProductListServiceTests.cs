using Wishlist.Core.Contracts;
using Wishlist.Core.Entities;
using Wishlist.Core.Exceptions;
using Wishlist.Functions.Services;

namespace Wishlist.Tests;

public class ProductListServiceTests
{
    [Fact]
    public async Task GetListsForCustomer_returns_only_that_customers_lists()
    {
        // Arrange: build the situation
        var repo = new FakeProductListRepository();
        repo.Lists.Add(new ProductList { Id = Guid.NewGuid(), Name = "A", CustomerId = "c1" });
        repo.Lists.Add(new ProductList { Id = Guid.NewGuid(), Name = "B", CustomerId = "c2" });
        var service = new ProductListService(repo);

        // Act: do the thing being tested
        var result = await service.GetListsForCustomerAsync("c1", CancellationToken.None);

        // Assert: check the outcome
        Assert.Single(result);
        Assert.Equal("A", result[0].Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateList_rejects_blank_name(string name)
    {
        var service = new ProductListService(new FakeProductListRepository());

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateListAsync(new CreateListRequest(name, "c1", Core.Enums.ProductListType.TYPE_WISH_LIST, true), CancellationToken.None));
    }

    [Fact]
    public async Task CreateList_saves_the_list()
    {
        var repo = new FakeProductListRepository();
        var service = new ProductListService(repo);

        await service.CreateListAsync(new CreateListRequest("Birthday", "c1", Core.Enums.ProductListType.TYPE_WISH_LIST, true), CancellationToken.None);

        Assert.Single(repo.Lists);
        Assert.Equal(1, repo.SaveCount);
    }
}