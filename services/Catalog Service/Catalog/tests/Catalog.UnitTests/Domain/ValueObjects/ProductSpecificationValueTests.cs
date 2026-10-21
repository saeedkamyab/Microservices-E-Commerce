using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.ValueObjects;

public class ProductSpecificationValueTests
{
    #region CreateText Tests
    [Fact]
    public void CreateText_Should_Create_ProductSpecificationValue_With_Valid_Text()
    {
        // Arrange
        var textValue = "Sample Text";
        // Act
        var productSpecificationValue = ProductSpecificationValue.CreateText(textValue);
        // Assert
        Assert.Equal(AttributeType.Text, productSpecificationValue.Type);
        Assert.Equal(textValue, productSpecificationValue.Value);
    }
    [Fact]
    public void CreateText_Should_Throw_Exception_When_Text_Is_Null_Or_Empty()
    {
        // Arrange
        string? nullText = null;

        string emptyText = "";

        string whitespaceText = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ProductSpecificationValue.CreateText(nullText!));
        Assert.Throws<ArgumentException>(() => ProductSpecificationValue.CreateText(emptyText));
        Assert.Throws<ArgumentException>(() => ProductSpecificationValue.CreateText(whitespaceText));
    }


    [Fact]
    public void CreateText_Should_Trim_Value()
    {
        var result =
            ProductSpecificationValue.CreateText("  Hello  ");

        Assert.Equal("Hello", result.Value);
    }

    #endregion

    #region CreateNumber Tests

    [Fact]
    public void CreateNumber_Should_Create_ProductSpecificationValue_With_Valid_Number()
    {
        // Arrange
        int numberValue = 42;
        // Act
        var productSpecificationValue = ProductSpecificationValue.CreateNumber(numberValue);
        // Assert
        Assert.Equal(AttributeType.Number, productSpecificationValue.Type);
        Assert.Equal(numberValue, productSpecificationValue.Value);
    }

    #endregion

    #region CreateDecimal Tests
    [Fact]
    public void CreateDecimal_Should_Create_ProductSpecificationValue_With_Valid_Decimal()
    {
        // Arrange
        decimal decimalValue = 42.5m;
        // Act
        var productSpecificationValue = ProductSpecificationValue.CreateDecimal(decimalValue);
        // Assert
        Assert.Equal(AttributeType.Decimal, productSpecificationValue.Type);
        Assert.Equal(decimalValue, productSpecificationValue.Value);
    }


    #endregion

    #region CreateBoolean Tests
    [Fact]
    public void CreateBoolean_Should_Create_ProductSpecificationValue_With_Valid_Boolean()
    {
        // Arrange
        bool booleanValue = true;
        // Act
        var productSpecificationValue = ProductSpecificationValue.CreateBoolean(booleanValue);
        // Assert
        Assert.Equal(AttributeType.Boolean, productSpecificationValue.Type);
        Assert.Equal(booleanValue, productSpecificationValue.Value);
    }


    #endregion

    #region CreateDate Tests
    [Fact]
    public void CreateDate_Should_Create_ProductSpecificationValue_With_Valid_Date()
    {
        // Arrange
        DateTime dateValue = new DateTime(2024, 1, 1);
        // Act
        var productSpecificationValue = ProductSpecificationValue.CreateDate(dateValue);
        // Assert
        Assert.Equal(AttributeType.Date, productSpecificationValue.Type);
        Assert.Equal(dateValue, productSpecificationValue.Value);
    }


    #endregion

    #region CreateOption Tests
    
    [Fact]
    public void CreateOption_Should_Create_ProductSpecificationValue_With_Valid_Option()
    {
        // Arrange
        var optionValue = AttributeOption.Create("Option 1");
        // Act
        var productSpecificationValue = ProductSpecificationValue.CreateOption(optionValue);
        // Assert
        Assert.Equal(AttributeType.Option, productSpecificationValue.Type);
        Assert.Equal(optionValue, productSpecificationValue.Value);
    }
    [Fact]
    public void CreateOption_Should_Throw_Exception_When_Option_Is_Null()
    {
        // Arrange
        AttributeOption? nullOption = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ProductSpecificationValue.CreateOption(nullOption!));
    }

    #endregion

}
