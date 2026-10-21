using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.ValueObjects;

public class CategoryNameTests
{

    #region CreateCategoryName Tests

    [Fact]
    public void Create_Should_Create_CategoryName_With_Valid_Value()
    {
        var name = "Electronics";
        var categoryName = CategoryName.Create(name);
        Assert.Equal(name, categoryName.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Throw_When_Value_Is_Invalid(
      string? value)
    {
        Assert.Throws<ArgumentException>(() =>
            CategoryName.Create(value!));
    }

    [Fact]
    public void Create_Should_Throw_Exception_When_Value_Exceeds_Max_Length()
    {
        var longName = new string('a', 101);
        Assert.Throws<ArgumentException>(() => CategoryName.Create(longName));
    }

    [Fact]
    public void Create_Should_Trim_Value()
    {
        var result =
            CategoryName.Create("  Hello  ");

        Assert.Equal("Hello", result.Value);
    }

    #endregion



}
