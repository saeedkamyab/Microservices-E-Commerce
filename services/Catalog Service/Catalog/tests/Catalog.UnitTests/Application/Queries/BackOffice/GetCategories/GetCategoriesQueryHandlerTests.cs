using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Category.Queries.BackOffice.GetCategories;
using Catalog.Application.Common;
using Catalog.Application.Common.Enums;
using Catalog.Application.ReadModels;
using Moq;
using static Catalog.Application.Common.Enums.SortEnum;

namespace Catalog.UnitTests.Application.Queries.BackOffice.GetCategories;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPagedResult()
    {
        // Arrange
        var mockCategoryReadService = new Mock<ICategoryReadService>();

        var query = new GetCategoriesQuery(
            Search: "test",
            Status: "Active",
            SortBy: "Name",
            SortDirection: "Asc",
            PageNumber: 1,
            PageSize: 10);

        var expectedResult = new PagedResult<CategoryReadModel>(
            items: new List<CategoryReadModel>
            {
                new CategoryReadModel(Guid.NewGuid(), "Category 1", "Active", null, null),
                new CategoryReadModel(Guid.NewGuid(), "Category 2", "Active", null, null)
            },
            totalCount: 2,
            pageNumber: 1,
            pageSize: 10);

            mockCategoryReadService
            .Setup(service => service.GetCategoriesAsync(
                It.IsAny<string>(),
                It.IsAny<CategoryStatusFilter?>(),
                It.IsAny<CategorySortBy>(),
                It.IsAny<SortDirection>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var handler = new GetCategoriesQueryHandler(mockCategoryReadService.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.TotalCount, result.TotalCount);
        Assert.Equal(expectedResult.PageNumber, result.PageNumber);
        Assert.Equal(expectedResult.PageSize, result.PageSize);
        Assert.Equal(expectedResult.Items.Count(), result.Items.Count());
    }

}
