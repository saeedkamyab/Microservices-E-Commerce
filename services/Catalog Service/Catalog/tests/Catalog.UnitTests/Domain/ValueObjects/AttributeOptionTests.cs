using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.ValueObjects;

public class AttributeOptionTests
{

    #region CreateAttributeOption Tests
    
    [Fact]
    public void Create_Should_Create_AttributeOption_With_Valid_Value()
    {
        var value = "Option 1";

        var attributeOption = AttributeOption.Create(value);

        Assert.Equal(value, attributeOption.Value);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Throw_When_Value_Is_Invalid(
   string? value)
    {
        Assert.Throws<ArgumentException>(() =>
            AttributeOption.Create(value!));
    }

    [Fact]
    public void Create_Should_Throw_Exception_When_Value_exceeds_Max_Length()
    {
        var longValue = new string('a', 101);
        Assert.Throws<ArgumentException>(() => AttributeOption.Create(longValue));
    }

    [Fact]
    public void Create_Should_Create_With_100_Character_Value()
    {
        var validValue = new string('a', 100);

        var attributeOption = AttributeOption.Create(validValue);

        Assert.Equal(validValue, attributeOption.Value);
    }

    [Fact]
    public void Create_Should_Trim_Value()
    {
        var result =
            AttributeOption.Create("  Hello  ");

        Assert.Equal("Hello", result.Value);
    }

    #endregion

}
