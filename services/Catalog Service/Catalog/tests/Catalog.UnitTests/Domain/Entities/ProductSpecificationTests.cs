using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.Entities;

public class ProductSpecificationTests
{
    [Fact]
    public void Create_Should_Reject_Empty_AttributeDefinitionId()
    {
        // Arrange
        var value = ProductSpecificationValue.CreateText("Example");

        // Act
        var action = () => ProductSpecification.Create(Guid.Empty, value);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_Should_Reject_Null_Value()
    {
        // Act
        var action = () => ProductSpecification.Create(Guid.NewGuid(), null!);

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void Create_Should_Create_ProductSpecification()
    {
        // Arrange
        var attributeDefinitionId = Guid.NewGuid();
        var value = ProductSpecificationValue.CreateText("Example");

        // Act
        var spec = ProductSpecification.Create(attributeDefinitionId, value);

        // Assert
        Assert.NotEqual(Guid.Empty, spec.Id);
        Assert.Equal(attributeDefinitionId, spec.AttributeDefinitionId);
        Assert.Equal(value, spec.Value);
    }

    [Fact]
    public void ChangeValue_Should_Change_Value()
    {
        // Arrange
        var spec = ProductSpecification.Create(Guid.NewGuid(), ProductSpecificationValue.CreateText("Old"));
        var newValue = ProductSpecificationValue.CreateText("New");

        // Act
        spec.ChangeValue(newValue);

        // Assert
        Assert.Equal(newValue, spec.Value);
    }

    [Fact]
    public void ChangeValue_Should_Reject_Null_Value()
    {
        // Arrange
        var spec = ProductSpecification.Create(Guid.NewGuid(), ProductSpecificationValue.CreateText("Old"));

        // Act
        var action = () => spec.ChangeValue(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }
}
