using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.ValueObjects;

public class AttributeOptionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Reject_Null_Or_Whitespace(string? input)
    {
        // Act
        var action = () => AttributeOption.Create(input!);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_Should_Trim_Value_And_Set_ToString()
    {
        // Arrange
        var raw = "  Blue  ";

        // Act
        var option = AttributeOption.Create(raw);

        // Assert
        Assert.Equal("Blue", option.Value);
        Assert.Equal("Blue", option.ToString());
    }

    [Fact]
    public void Create_Should_Allow_Exactly_100_Characters()
    {
        // Arrange
        var value = new string('a', 100);

        // Act
        var option = AttributeOption.Create(value);

        // Assert
        Assert.Equal(value, option.Value);
    }

    [Fact]
    public void Create_Should_Reject_More_Than_100_Characters()
    {
        // Arrange
        var value = new string('a', 101);

        // Act
        var action = () => AttributeOption.Create(value);

        // Assert
        var ex = Assert.Throws<ArgumentException>(action);
        Assert.Equal("value", ex.ParamName);
    }

    [Fact]
    public void Equality_Is_Value_Based_For_Same_Text()
    {
        // Arrange & Act
        var a = AttributeOption.Create("Blue");
        var b = AttributeOption.Create("Blue");

        // Assert
        Assert.Equal(a, b);
    }
}
