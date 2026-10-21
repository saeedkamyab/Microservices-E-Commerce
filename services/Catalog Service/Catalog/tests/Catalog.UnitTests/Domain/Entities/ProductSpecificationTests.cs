using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.Entities;

public class ProductSpecificationTests
{
    #region Create Tests
    [Fact]
    public void Create_ShouldReturnProductSpecification_When_ValidParameters()
    {
        // Arrange
        var attributeDefinitionId = Guid.NewGuid();
        var value = ProductSpecificationValue.CreateText("Test Value");
       
        // Act
        var specification = ProductSpecification.Create(attributeDefinitionId, value);
       
        // Assert
        Assert.NotNull(specification);
        Assert.Equal(attributeDefinitionId, specification.AttributeDefinitionId);
        Assert.Equal(value, specification.Value);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_When_AttributeDefinitionId_Is_Empty()
    {
        // Arrange
        var attributeDefinitionId = Guid.Empty;

        var value = ProductSpecificationValue.CreateText("Test Value");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ProductSpecification.Create(attributeDefinitionId, value));
     
    }

    [Fact]
    public void Create_Should_Throw_ArgumentNullException_When_Value_Is_Null()
    {
        // Arrange
        var attributeDefinitionId = Guid.NewGuid();
      
        ProductSpecificationValue value = null!;
      
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => ProductSpecification.Create(attributeDefinitionId, value));

    }

    #endregion

    #region ChangeValue Tests
    [Fact]
    public void ChangeValue_Should_Update_Value_When_ValidValue()
    {
        // Arrange
        var attributeDefinitionId = Guid.NewGuid();

        var initialValue = ProductSpecificationValue.CreateText("Initial Value");

        var specification = ProductSpecification.Create(attributeDefinitionId, initialValue);

        var newValue = ProductSpecificationValue.CreateText("New Value");

        // Act
        specification.ChangeValue(newValue);
        // Assert
        Assert.Equal(newValue, specification.Value);
    }

    [Fact]
    public void ChangeValue_Should_Throw_ArgumentNullException_When_Value_Is_Null()
    {
        // Arrange
        var attributeDefinitionId = Guid.NewGuid();

        var initialValue = ProductSpecificationValue.CreateText("Initial Value");

        var specification = ProductSpecification.Create(attributeDefinitionId, initialValue);

        ProductSpecificationValue newValue = null!;
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => specification.ChangeValue(newValue));
    }

    #endregion

    #region Rehydrate Tests

    [Fact]
    public void Rehydrate_Should_Return_ProductSpecification_When_ValidParameters()
    {
        // Arrange
        var id = Guid.NewGuid();

        var attributeDefinitionId = Guid.NewGuid();

        var value = ProductSpecificationValue.CreateText("Test Value");
        // Act
        var specification = ProductSpecification.Rehydrate(id, attributeDefinitionId, value);
        // Assert
        Assert.NotNull(specification);
        Assert.Equal(id, specification.Id);
        Assert.Equal(attributeDefinitionId, specification.AttributeDefinitionId);
        Assert.Equal(value, specification.Value);
    }

    [Fact]
    public void Rehydrate_Should_Throw_ArgumentException_When_Id_Is_Empty()
    {
        // Arrange
        var id = Guid.Empty;

        var attributeDefinitionId = Guid.NewGuid();

        var value = ProductSpecificationValue.CreateText("Test Value");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ProductSpecification.Rehydrate(id, attributeDefinitionId, value));
    }

    [Fact]
    public void Rehydrate_Should_Throw_ArgumentException_When_AttributeDefinitionId_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();

        var attributeDefinitionId = Guid.Empty;

        var value = ProductSpecificationValue.CreateText("Test Value");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ProductSpecification.Rehydrate(id, attributeDefinitionId, value));
    }

    [Fact]
    public void Rehydrate_Should_Throw_ArgumentNullException_When_Value_Is_Null()
    {
        // Arrange
        var id = Guid.NewGuid();

        var attributeDefinitionId = Guid.NewGuid();

        ProductSpecificationValue value = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => ProductSpecification.Rehydrate(id, attributeDefinitionId, value));
    }

    #endregion

}
