using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.Entities;

public class ProductTests
{
    #region CreateProduct Tests
    [Fact]
    public void CreateProduct_Should_CreateProduct_When_ValidParameters()
    {
        // Arrange
        var name = ProductName.Create("Test Product");

        var description = "This is a test product.";

        var categoryId = Guid.NewGuid();

        var price = Price.Create(10.99m);

        // Act
        var product = Product.Create(name, description, categoryId, price);

        // Assert
        Assert.NotNull(product);
        Assert.Equal(name, product.Name);
        Assert.Equal(description, product.ProductDescription);
        Assert.Equal(categoryId, product.CategoryId);
        Assert.Equal(price, product.Price);
        Assert.Equal(ProductStatus.Draft, product.Status);
    }

    [Fact]
    public void CreateProduct_Should_Throw_ArgumentException_When_CategoryIdIsEmpty()
    {
        // Arrange
        var name = ProductName.Create("Test Product");

        var description = "This is a test product.";

        var categoryId = Guid.Empty;

        var price = Price.Create(10.99m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Product.Create(name, description, categoryId, price));
    }
    [Fact]
    public void CreateProduct_Should_Throw_ArgumentNullException_When_NameIsNull()
    {
        // Arrange
        ProductName name = null!;

        var description = "This is a test product.";

        var categoryId = Guid.NewGuid();

        var price = Price.Create(10.99m);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Product.Create(name, description, categoryId, price));
    }
    [Fact]
    public void CreateProduct_Should_Throw_ArgumentNullException_When_PriceIsNull()
    {
        // Arrange
        var name = ProductName.Create("Test Product");

        var description = "This is a test product.";

        var categoryId = Guid.NewGuid();

        Price price = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Product.Create(name, description, categoryId, price));
    }


    #endregion

    #region Rename Tests
    [Fact]
    public void Rename_Should_ChangeName_When_ValidNewName()
    {
        var product = CreateValidProduct();

        var newName = ProductName.Create("Updated Product");
        // Act
        product.Rename(newName);
        // Assert
        Assert.Equal(newName, product.Name);
    }

    [Fact]
    public void Rename_Should_Throw_ArgumentNullException_When_NewName_IsNull()
    {
        var product = CreateValidProduct();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => product.Rename(null!));
    }
    #endregion

    #region ChangeCategory Tests
    [Fact]
    public void ChangeCategory_Should_ChangeCategory_When_ValidNewCategoryId()
    {
        var product = CreateValidProduct();

        var newCategoryId = Guid.NewGuid();
        // Act
        product.ChangeCategory(newCategoryId);
        // Assert
        Assert.Equal(newCategoryId, product.CategoryId);
    }
    [Fact]
    public void ChangeCategory_Should_Throw_ArgumentException_When_NewCategoryId_Is_Empty()
    {
        var product = CreateValidProduct();
        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.ChangeCategory(Guid.Empty));
    }


    #endregion

    #region ChangePrice Tests
    [Fact]
    public void ChangePrice_Should_ChangePrice_When_ValidNewPrice()
    {
        var product = CreateValidProduct();

        var newPrice = Price.Create(15.99m);

        // Act
        product.ChangePrice(newPrice);
        // Assert
        Assert.Equal(newPrice, product.Price);
    }
    [Fact]
    public void ChangePrice_Should_Throw_ArgumentNullException_When_NewPriceIsNull()
    {
        var product = CreateValidProduct();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => product.ChangePrice(null!));
    }
    [Fact]
    public void ChangePrice_Should_NotChangePrice_When_New_Price_Is_Same_As_CurrentPrice()
    {
        // Arrange
        var product = CreateValidProduct();
        var price = Price.Create(10.99m);

        // Act
        product.ChangePrice(price);
        // Assert
        Assert.Equal(price, product.Price);
    }

    #endregion

    #region Activate Tests
    [Fact]
    public void Activate_Should_ChangeStatusToActive_When_Product_Is_Draft()
    {
        var product = CreateValidProduct();
        // Act
        product.Activate();
        // Assert
        Assert.Equal(ProductStatus.Active, product.Status);
    }
    [Fact]
    public void Activate_Should_Throw_InvalidOperationException_When_Product_Amount_Is_Zero()
    {
        var product = CreateValidProduct();
        product.ChangePrice(Price.Create(0));

        Assert.Throws<InvalidOperationException>(() => product.Activate());

    }

    #endregion

    #region Deactivate Tests
    [Fact]
    public void Deactivate_Should_Change_Status_To_Inactive_When_Product_Is_Active()
    {
        var product = CreateValidProduct();

        product.Activate();
        // Act
        product.Deactivate();
        // Assert
        Assert.Equal(ProductStatus.Inactive, product.Status);
    }
    [Fact]
    public void Deactivate_Should_Throw_InvalidOperationException_When_Product_Is_Draft()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.Deactivate());
    }

    #endregion

    #region AddSpecification Tests
    [Fact]
    public void AddSpecification_Should_Add_Specification_When_ValidSpecification()
    {
        var product = CreateValidProduct();

        var attributeDefinitionId = Guid.NewGuid();

        var specification = ProductSpecification.Create(
            attributeDefinitionId, ProductSpecificationValue.CreateText("Red"));

        // Act
        product.AddSpecification(specification);
        // Assert
        Assert.Contains(specification, product.Specifications);
    }
    [Fact]
    public void AddSpecification_Should_Throw_InvalidOperationException_When_SpecificationAlreadyExists()
    {
        // Arrange
        var product = CreateValidProduct();

        var attributeDefinitionId = Guid.NewGuid();

        var specification1 = ProductSpecification.Create(
            attributeDefinitionId, ProductSpecificationValue.CreateText("Red"));
        
        var specification2 = ProductSpecification.Create(
            attributeDefinitionId, ProductSpecificationValue.CreateText("Blue"));

        product.AddSpecification(specification1);
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.AddSpecification(specification2));
    }
    #endregion

    #region ChangeSpecification Tests

    [Fact]
    public void ChangeSpecification_Should_Update_Value_When_Specification_Exists()
    {
        // Arrange
        var product = CreateValidProduct();
        var attributeDefinitionId = Guid.NewGuid();

        var oldValue = ProductSpecificationValue.CreateText("Red");
        var newValue = ProductSpecificationValue.CreateText("Blue");

        var specification = ProductSpecification.Create(
           attributeDefinitionId, oldValue);

        product.AddSpecification(specification);

        // Act
        product.ChangeSpecification(attributeDefinitionId, newValue);

        // Assert
        var specificationItem = Assert.Single(product.Specifications);

        Assert.Equal(attributeDefinitionId, specification.AttributeDefinitionId);
        Assert.Equal(newValue, specificationItem.Value);
    }
    [Fact]
    public void ChangeSpecification_Should_Throw_When_Specification_Does_Not_Exist()
    {
        // Arrange
        var product = CreateValidProduct();
        var attributeDefinitionId = Guid.NewGuid();

        var newValue = ProductSpecificationValue.CreateText("Blue");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.ChangeSpecification(attributeDefinitionId, newValue));
    }

    #endregion

    #region RemoveSpecification Tests

    [Fact]
    public void RemoveSpecification_Should_Remove_Specification_When_Specification_Exists()
    {
        // Arrange
        var product = CreateValidProduct();

        var attributeDefinitionId = Guid.NewGuid();

        var specification = ProductSpecification.Create(
            attributeDefinitionId, ProductSpecificationValue.CreateText("Red"));

        product.AddSpecification(specification);
        // Act
        product.RemoveSpecification(attributeDefinitionId);
        // Assert
        Assert.DoesNotContain(specification, product.Specifications);
    }

    #endregion



    private Product CreateValidProduct()
    {
        var name = ProductName.Create("Test Product");

        var description = "This is a test product.";

        var categoryId = Guid.NewGuid();

        var price = Price.Create(10.99m);

        return Product.Create(name, description, categoryId, price);
    }
}
