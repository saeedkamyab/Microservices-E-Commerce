using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.ValueObjects;

public class NameTests
{
    #region Create Tests
    [Fact]
    public void Create_ValidName_Should_CreateNameObject()
    {
        // Arrange
        var validName = "Valid Name";

        // Act
        var name = Name.Create(validName);

        // Assert
        Assert.Equal(validName, name.Value);
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidName_Should_ThrowArgumentException(string? invalidName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Name.Create(invalidName!));
    }

    [Fact]
    public void Create_Name_Exceeding_MaxLength_Should_ThrowArgumentException()
    {
        // Arrange
        var longName = new string('a', 101); // 101 characters
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Name.Create(longName));
    }

    #endregion
}
