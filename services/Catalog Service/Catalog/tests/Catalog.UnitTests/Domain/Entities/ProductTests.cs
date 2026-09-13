using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Create_Should_Reject_Empty_CategoryId()
    {
        var name = ProductName.Create("Phone");
        var price = Price.Create(100);

        var action = () => Product.Create(name, "desc", Guid.Empty, price);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_Should_Create_Draft_Product()
    {
        // Arrange
        var name = ProductName.Create("Phone");
        var price = Price.Create(199.99m);
        var categoryId = Guid.NewGuid();

        // Act
        var product = Product.Create(name, "A product", categoryId, price);

        // Assert
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal(name, product.Name);
        Assert.Equal("A product", product.ProductDescription);
        Assert.Equal(categoryId, product.CategoryId);
        Assert.Equal(price, product.Price);
        Assert.Equal(ProductStatus.Draft, product.Status);
    }

    [Fact]
    public void ChangePrice_Should_Throw_On_Null()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(10));

        Assert.Throws<ArgumentNullException>(() => product.ChangePrice(null!));
    }

    [Fact]
    public void ChangePrice_Should_Do_Nothing_When_Same_Price()
    {
        var price = Price.Create(50);
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            price);

        product.ChangePrice(Price.Create(50)); // equal by record semantics

        Assert.Equal(price, product.Price);
    }

    [Fact]
    public void ChangePrice_Should_Update_Price()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(50));

        var newPrice = Price.Create(75);

        product.ChangePrice(newPrice);

        Assert.Equal(newPrice, product.Price);
    }

    [Fact]
    public void Activate_Should_Throw_When_Price_Is_Invalid()
    {
        var product = Product.Create(
            ProductName.Create("Freebie"),
            null,
            Guid.NewGuid(),
            Price.Create(0));

        var action = () => product.Activate();

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Activate_Should_Set_Status_To_Active()
    {
        var product = Product.Create(
            ProductName.Create("Paid"),
            null,
            Guid.NewGuid(),
            Price.Create(20));

        product.Activate();

        Assert.Equal(ProductStatus.Active, product.Status);
    }

    [Fact]
    public void Deactivate_Should_Throw_When_Status_Is_Draft()
    {
        var product = Product.Create(
            ProductName.Create("Item"),
            null,
            Guid.NewGuid(),
            Price.Create(10));

        Assert.Throws<InvalidOperationException>(() => product.Deactivate());
    }

    [Fact]
    public void Deactivate_Should_Set_Status_To_Inactive_From_Active()
    {
        var product = Product.Create(
            ProductName.Create("Item"),
            null,
            Guid.NewGuid(),
            Price.Create(10));

        product.Activate();
        product.Deactivate();

        Assert.Equal(ProductStatus.Inactive, product.Status);
    }

    [Fact]
    public void AddSpecification_Should_Add_Specification()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        var spec = ProductSpecification.Create(
            Guid.NewGuid(),
            ProductSpecificationValue.CreateText("Value"));

        product.AddSpecification(spec);

        var result = Assert.Single(product.Specifications);
        Assert.Equal(spec, result);
    }

    [Fact]
    public void AddSpecification_Should_Reject_Null()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        Assert.Throws<ArgumentNullException>(() => product.AddSpecification(null!));
    }

    [Fact]
    public void AddSpecification_Should_Reject_Duplicate_Attribute()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        var attributeId = Guid.NewGuid();

        product.AddSpecification(
            ProductSpecification.Create(attributeId, ProductSpecificationValue.CreateText("v1")));

        Assert.Throws<InvalidOperationException>(() =>
            product.AddSpecification(
                ProductSpecification.Create(attributeId, ProductSpecificationValue.CreateText("v2"))));
    }

    [Fact]
    public void ChangeSpecification_Should_Throw_When_Not_Exist()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        Assert.Throws<InvalidOperationException>(() =>
            product.ChangeSpecification(Guid.NewGuid(), ProductSpecificationValue.CreateText("x")));
    }

    [Fact]
    public void ChangeSpecification_Should_Update_Value()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        var attrId = Guid.NewGuid();
        var spec = ProductSpecification.Create(attrId, ProductSpecificationValue.CreateText("old"));
        product.AddSpecification(spec);

        product.ChangeSpecification(attrId, ProductSpecificationValue.CreateText("new"));

        var result = Assert.Single(product.Specifications);
        Assert.Equal("new", result.Value.Value);
    }

    [Fact]
    public void RemoveSpecification_Should_Remove_When_Exists()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        var attrId = Guid.NewGuid();
        var spec = ProductSpecification.Create(attrId, ProductSpecificationValue.CreateText("v"));
        product.AddSpecification(spec);

        product.RemoveSpecification(attrId);

        Assert.Empty(product.Specifications);
    }

    [Fact]
    public void RemoveSpecification_With_Unknown_Id_Should_Do_Nothing()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        // nothing added
        product.RemoveSpecification(Guid.NewGuid());

        Assert.Empty(product.Specifications);
    }
    [Fact]
    public void Create_Should_Reject_Null_Name()
    {
        var action = () => Product.Create(
            null!,
            null,
            Guid.NewGuid(),
            Price.Create(100));

        Assert.Throws<ArgumentNullException>(action);
    }
    [Fact]
    public void Create_Should_Reject_Null_Price()
    {
        var action = () => Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            null!);

        Assert.Throws<ArgumentNullException>(action);
    }
    [Fact]
    public void Activate_When_Already_Active_Should_Do_Nothing()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        product.Activate();

        product.Activate();

        Assert.Equal(ProductStatus.Active, product.Status);
    }
    [Fact]
    public void ChangeSpecification_Should_Reject_Null_Value()
    {
        var product = Product.Create(
            ProductName.Create("Phone"),
            null,
            Guid.NewGuid(),
            Price.Create(100));

        var attrId = Guid.NewGuid();

        product.AddSpecification(
            ProductSpecification.Create(
                attrId,
                ProductSpecificationValue.CreateText("old")));

        Assert.Throws<ArgumentNullException>(() =>
            product.ChangeSpecification(attrId, null!));
    }
}