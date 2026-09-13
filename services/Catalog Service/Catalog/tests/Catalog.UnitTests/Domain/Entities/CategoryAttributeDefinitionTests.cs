using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;
using System.Xml.Linq;

namespace Catalog.UnitTests.Domain.Entities;

public class CategoryAttributeDefinitionTests
{

    [Fact]
    public void Create_Should_Create_CategoryAttributeDefinition()
    {
        //Arrange
        var name = Name.Create("Ram");

        //Act
        var attributeDefinition = CategoryAttributeDefinition.Create(name, AttributeType.Text, true);

        //Assert
        Assert.NotEqual(Guid.Empty, attributeDefinition.Id);
        Assert.Equal(name, attributeDefinition.Name);
        Assert.Equal(AttributeType.Text, attributeDefinition.Type);
        Assert.True(attributeDefinition.IsRequired);

    }
    [Fact]
    public void Create_Should_Reject_Null_Name()
    {
        var action = () => CategoryAttributeDefinition.Create(null!, AttributeType.Text, true);
        Assert.Throws<ArgumentNullException>(action);
    }
   
    [Fact]
    public void AddOption_Should_Add_AttributeOption()
    {
        //Arrange
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"),AttributeType.Option,true);
        var attributeOption = AttributeOption.Create("Blue");
        //Act
        attributeDefinition.AddOption(attributeOption);

        //Assert
        var result = Assert.Single(attributeDefinition.Options);
        Assert.Equal(attributeOption,result);
    }

    [Fact]
    public void AddOption_Should_Reject_Null_Option()
    {
        //Arrange
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Option, true);

        //Act
        var action = () => attributeDefinition.AddOption(null!);

        //Assert
        Assert.Throws<ArgumentNullException>(action);
    }
    [Fact]
    public void AddOption_Should_Reject_NonOption()
    {
        //Arrange
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Decimal, true);

        //Act
        var action = () => attributeDefinition.AddOption(AttributeOption.Create("Red"));


        //Assert
        Assert.Throws<InvalidOperationException>(action);
    }
    [Fact]
    public void AddOption_Should_Reject_Duplicate_Option()
    {
        //Arrange
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Option, true);
        var attributeOption1 = AttributeOption.Create("Blue");
        var attributeOption2 = AttributeOption.Create("Blue");

        //Act
        attributeDefinition.AddOption(attributeOption1);
        attributeDefinition.AddOption(attributeOption2);

        var result = Assert.Single(attributeDefinition.Options);
        Assert.Equal(attributeOption1, result);
    }

    [Fact]
    public void RemoveOption_Should_Remove_AttributeOption()
    {
        //Arrange
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Option, true);
        var attributeOption = AttributeOption.Create("Blue");
        //Act
        attributeDefinition.AddOption(attributeOption);

        attributeDefinition.RemoveOption(attributeOption);
        //Assert
        Assert.Empty(attributeDefinition.Options);
      
    }

    [Fact]
    public void RemoveOption_Should_Reject_Null_Option()
    {
        //Arrange
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Option, true);
        var attributeOption = AttributeOption.Create("Blue");

        //Act
        var action = () => attributeDefinition.RemoveOption(null!);
       
        //Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void RemoveOption_Should_Reject_NonOption()
    {
        //Arrange
        var attributeDefinition = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Decimal, true);

        //Act
        var action = () => attributeDefinition.RemoveOption(AttributeOption.Create("Red"));


        //Assert
        Assert.Throws<InvalidOperationException>(action);
    }
    [Fact]
    public void ValidateValue_With_Matching_Type_Should_Not_Throw()
    {
        // Arrange
        var definition = CategoryAttributeDefinition.Create(
            Name.Create("RAM"),
            AttributeType.Number,
            true);

        var value = ProductSpecificationValue.CreateNumber(8);

        // Act
        var exception = Record.Exception(
            () => definition.ValidateValue(value));

        // Assert
        Assert.Null(exception);
    }
    [Fact]
    public void ValidateValue_With_Different_Type_Should_Throw()
    {
        // Arrange
        var definition = CategoryAttributeDefinition.Create(
            Name.Create("RAM"),
            AttributeType.Number,
            true);

        var value = ProductSpecificationValue.CreateText("8");

        // Act
        var action = () => definition.ValidateValue(value);

        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void ValidateValue_With_Null_Value_Should_Throw()
    {
        // Arrange
        var definition = CategoryAttributeDefinition.Create(
            Name.Create("RAM"),
            AttributeType.Number,
            true);

        // Act
        var action = () =>
            definition.ValidateValue(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void ValidateValue_With_Valid_Option_Should_Not_Throw()
    {
        // Arrange
        var definition = CategoryAttributeDefinition.Create(
            Name.Create("Color"),
            AttributeType.Option,
            true);

        var black = AttributeOption.Create("Black");

        definition.AddOption(black);

        var value =
            ProductSpecificationValue.CreateOption(black);

        // Act
        var exception = Record.Exception(
            () => definition.ValidateValue(value));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ValidateValue_With_Invalid_Option_Should_Throw()
    {
        // Arrange
        var definition = CategoryAttributeDefinition.Create(
            Name.Create("Color"),
            AttributeType.Option,
            true);

        var black = AttributeOption.Create("Black");
        var white = AttributeOption.Create("White");

        definition.AddOption(black);

        var value =
            ProductSpecificationValue.CreateOption(white);

        // Act
        var action = () =>
            definition.ValidateValue(value);

        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }

}
