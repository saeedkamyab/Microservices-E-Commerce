using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Category.Coammands.CreateCategory;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Moq;

namespace Catalog.UnitTests.Application.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenParentCategoryNotFound()
    {
        // Arrange
        var parentId = Guid.NewGuid();

        var repoMock = new Mock<ICategoryRepository>();
        repoMock
            .Setup(r => r.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var uowMock = new Mock<IUnitOfWork>();

        var handler = new CreateCategoryCommandHandler(repoMock.Object, uowMock.Object);

        var command = new CreateCategoryCommand(
            Name: "Any",
            ParentCategoryId: parentId,
            Attributes: Array.Empty<CategoryAttributeDefinitionInput>());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));

        repoMock.Verify(r => r.GetByIdAsync(parentId, It.IsAny<CancellationToken>()), Times.Once);
        repoMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldAddCategoryAndSave_WhenRequestIsValid()
    {
        // Arrange
        var repoMock = new Mock<ICategoryRepository>();
        Category? capturedCategory = null;

        repoMock
            .Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Callback<Category, CancellationToken>((c, ct) => capturedCategory = c)
            .Returns(Task.CompletedTask);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(1);

        var handler = new CreateCategoryCommandHandler(repoMock.Object, uowMock.Object);

        var attributeInput = new CategoryAttributeDefinitionInput(
            Name: "Color",
            Type: AttributeType.Option,
            IsRequired: true,
            Options: new[] { "Red", "Green" });

        var command = new CreateCategoryCommand(
            Name: "Electronics",
            ParentCategoryId: null,
            Attributes: new[] { attributeInput });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        repoMock.Verify(
    r => r.GetByIdAsync(
        It.IsAny<Guid>(),
        It.IsAny<CancellationToken>()),
    Times.Never);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(capturedCategory);
        Assert.Equal(result, capturedCategory!.Id);

        // Verify category details
        Assert.Equal("Electronics", capturedCategory.CategoryName.Value);
        Assert.Null(capturedCategory.ParentCategoryId);

        var defs = capturedCategory.AttributeDefinitions.ToList();
        Assert.Single(defs);
        var def = defs[0];
        Assert.Equal("Color", def.Name.Value);
        Assert.Equal(AttributeType.Option, def.Type);
        Assert.True(def.IsRequired);
        Assert.Equal(2, def.Options.Count);
        var optionValues = def.Options.Select(o => o.Value).OrderBy(x => x).ToArray();
        Assert.Equal(new[] { "Green", "Red" }, optionValues);
    }
}


