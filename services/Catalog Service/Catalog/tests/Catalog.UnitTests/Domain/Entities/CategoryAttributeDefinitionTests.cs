using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.Entities;

public class CategoryAttributeDefinitionTests
{
    #region Create CategoryAttributeDefinition Tests
    [Fact]
    public void CreateCategoryAttributeDefinition_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = Name.Create("Brand");
        var type = AttributeType.Text;
        var isRequired = true;
        // Act
        var attributeDefinition = CategoryAttributeDefinition.Create(name, type, isRequired);
        // Assert
        Assert.NotNull(attributeDefinition);
        Assert.Equal(name, attributeDefinition.Name);
        Assert.Equal(type, attributeDefinition.Type);
        Assert.Equal(isRequired, attributeDefinition.IsRequired);
        Assert.Empty(attributeDefinition.Options);
    }
    [Fact]
    public void CreateCategoryAttributeDefinition_ShouldThrowException_WhenNameIsNull()
    {
        // Arrange
        Name name = null!;

        var type = AttributeType.Text;

        var isRequired = true;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            CategoryAttributeDefinition.Create(name, type, isRequired));
    }



    #endregion

    #region AddOption tests

    [Fact]
    public void AddOption_Should_Add_Option_Successfully()
    {
       
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Option, true);

        var option = AttributeOption.Create("Red");
      
        attributeDefinition.AddOption(option);
       
        Assert.Single(attributeDefinition.Options);
        Assert.Contains(option, attributeDefinition.Options);
    }

    [Fact]
    public void AddOption_Should_ThrowException_WhenAttributeTypeIsNotOption()
    {

        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Brand"), AttributeType.Text, true);
       
        var option = AttributeOption.Create("Red");

        Assert.Throws<InvalidOperationException>(() =>
            attributeDefinition.AddOption(option));
    }

    [Fact]
    public void AddOption_Should_NotAddDuplicateOption()
    {

        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Option, true);

        var option = AttributeOption.Create("Red");

        attributeDefinition.AddOption(option);
        attributeDefinition.AddOption(option);

        Assert.Single(attributeDefinition.Options);
    }


    #endregion

    #region RemoveOption tests
    [Fact]
    public void RemoveOption_Should_Remove_Option_Successfully()
    {
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Option, true);

        var option = AttributeOption.Create("Red");

        attributeDefinition.AddOption(option);

        attributeDefinition.RemoveOption(option);

        Assert.Empty(attributeDefinition.Options);
    }

    [Fact]
    public void RemoveOption_Should_ThrowException_When_AttributeType_Is_Not_Option()
    {
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Brand"), AttributeType.Text, true);

        var option = AttributeOption.Create("Red");

        Assert.Throws<InvalidOperationException>(() =>
            attributeDefinition.RemoveOption(option));
    }

    [Fact]
    public void RemoveOption_Should_ThrowException_When_Option_IsNull()
    {
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Option, true);

        Assert.Throws<ArgumentNullException>(() =>
            attributeDefinition.RemoveOption(null!));
    }
    #endregion


}
