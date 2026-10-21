using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.ValueObjects;

public class PriceTests
{

    #region Create Tests
    [Fact]
    public void Create_WithValidParameters_ReturnsPrice()
    {
        // Arrange
        var amount = 10.99m;

        // Act
        var price = Price.Create(amount);

        // Assert
        Assert.Equal(amount, price.Amount);

    }
    [Fact]
    public void Create_WithNegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var amount = -5.00m;
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Price.Create(amount));
    }

    #endregion


}
