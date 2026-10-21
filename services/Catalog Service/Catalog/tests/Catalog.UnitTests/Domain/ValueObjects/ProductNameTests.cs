using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.ValueObjects;

public class ProductNameTests
{

    #region CreateProductName Tests

    [Fact]
    public void Create_Should_Create_ProductName_With_Valid_Value()
    {
        var name = "Iphone";
        var productName = ProductName.Create(name);
        Assert.Equal(name, productName.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Throw_When_Value_Is_Invalid(
      string? value)
    {
        Assert.Throws<ArgumentException>(() =>
            ProductName.Create(value!));
    }

    [Fact]
    public void Create_Should_Throw_Exception_When_Value_Exceeds_Max_Length()
    {
        var longName = new string('a', 201);
        Assert.Throws<ArgumentException>(() => ProductName.Create(longName));
    }

    [Fact]
    public void Create_Should_Trim_Value()
    {
        var result =
            ProductName.Create("  IPHONE  ");

        Assert.Equal("IPHONE", result.Value);
    }

    #endregion

}
