using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Category.Queries.BackOffice.GetCategoryById;
using Moq;

namespace Catalog.UnitTests.Application.Queries.BackOffice.GetCategories;

public class GetCategoryByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCategoryDetailsResult_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var expectedResult = new CategoryDetailsResult(
            Id: categoryId,
            Name: "Test Category",
            ParentCategoryId: null,
            Status: "Active",
            Attributes:
            [
                new CategoryAttributeResult(
                Id: Guid.NewGuid(),
                Name: "Attribute 1",
                Type: "text",
                IsRequired: true,
                Options: []
            ),
            new CategoryAttributeResult(
                Id: Guid.NewGuid(),
                Name: "Attribute 2",
                Type: "text",
                IsRequired: true,
                Options: []
            )
            ]
        );

        var mockCategoryReadService = new Mock<ICategoryReadService>();

        mockCategoryReadService
            .Setup(service => service.GetByIdAsync(
                categoryId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var handler = new GetCategoryByIdQueryHandler(
            mockCategoryReadService.Object);

        var query = new GetCategoryByIdQuery(categoryId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.Name, result.Name);
        Assert.Equal(expectedResult.ParentCategoryId, result.ParentCategoryId);
        Assert.Equal(expectedResult.Status, result.Status);
        Assert.Equal(expectedResult.Attributes.Count, result.Attributes.Count);
    }
}
