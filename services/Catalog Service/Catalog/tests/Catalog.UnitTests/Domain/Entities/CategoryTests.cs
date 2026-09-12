using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_Should_Create_Active_Category()
    {
        //Arrange
        var name = CategoryName.Create("Mobile");

        //Act
        var category = Category.Create(name);


        // Assert

        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal(name, category.CategoryName);
        Assert.Null(category.ParentCategoryId);
        Assert.Equal(CategoryStatus.Active, category.Status);

    }
    [Fact]
    public void Create_Should_Create_Child_Category()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var name = CategoryName.Create("Mobile");

        // Act
        var category = Category.Create(
            name,
            parentId);

        // Assert
        Assert.Equal(parentId, category.ParentCategoryId);
    }
    [Fact]
    public void Create_Should_Reject_Null_CategoryName()
    {
        var action = () => Category.Create(null!);
        Assert.Throws<ArgumentNullException>(action);
    }
    [Fact]
    public void Rename_Should_Change_Category_Name()
    {
        //Arrange
        var category = Category.Create(CategoryName.Create("Mobile"));

        var newName = CategoryName.Create("SmartPhone");

        //Act
        category.Rename(newName);

        //Assert
        Assert.Equal(newName, category.CategoryName);
    }
    [Fact]
    public void Rename_Should_Reject_Null_Name()
    {
        var category = Category.Create(
            CategoryName.Create("Mobile"));

        var action = () => category.Rename(null!);

        Assert.Throws<ArgumentNullException>(action);
    }
    [Fact]
    public void Rename_With_Same_Name_Should_Do_Nothing()
    {
        // Arrange
        var name = CategoryName.Create("Mobile");

        var category = Category.Create(name);

        // Act
        category.Rename(
            CategoryName.Create("Mobile"));

        // Assert
        Assert.Equal(name, category.CategoryName);
    }
    [Fact]
    public void Deactivate_Should_Change_Status_To_Inactive()
    {
        //Arrange
        var category = Category.Create(CategoryName.Create("Mobile"));

        //Act
        category.Deactivate();

        //Assert
        Assert.Equal(CategoryStatus.Inactive, category.Status);
    }
    [Fact]
    public void Activate_Should_Change_Inactive_Category_To_Active()
    {
        // Arrange
        var category = Category.Create(
            CategoryName.Create("Mobile"));

        category.Deactivate();

        // Act
        category.Activate();

        // Assert
        Assert.Equal(
            CategoryStatus.Active,
            category.Status);
    }
    [Fact]
    public void AddAttributeDefinition_Should_Add_Definition()
    {
        //Arrange
        var category =
               Category.Create(
                   CategoryName.Create("Mobile"));

        var definition =
            CategoryAttributeDefinition.Create(Name.Create("RAM"),
                AttributeType.Number,
                true);
        //Act
        category.AddAttributeDefinition(definition);


        //Assert
        var result = Assert.Single(category.AttributeDefinitions);
        Assert.Equal(definition, result);
    }
    [Fact]
    public void AddAttributeDefinition_Should_Reject_Duplicate_Name()
    {
        //Arrange
        var category =
            Category.Create(
                CategoryName.Create("Mobile"));
        //Act
        category.AddAttributeDefinition(
            CategoryAttributeDefinition.Create(
                Name.Create("RAM"),
                AttributeType.Number,
                true));

        var action = () =>
            category.AddAttributeDefinition(
                CategoryAttributeDefinition.Create(
                    Name.Create("ram"),
                    AttributeType.Number,
                    true));
        //Assert
        Assert.Throws<InvalidOperationException>(action);
    }
    [Fact]
    public void AddAttributeDefinition_Should_Reject_Null_Definition()
    {
        var category = Category.Create(
            CategoryName.Create("Mobile"));

        var action = () => category.AddAttributeDefinition(null!);

        Assert.Throws<ArgumentNullException>(action);
    }
    [Fact]
    public void RemoveAttributeDefinition_Should_Remove_Definition()
    {
        var category = Category.Create(
            CategoryName.Create("Mobile"));

        var definition = CategoryAttributeDefinition.Create(
            Name.Create("RAM"),
            AttributeType.Number,
            true);

        category.AddAttributeDefinition(definition);

        category.RemoveAttributeDefinition(definition.Id);

        Assert.Empty(category.AttributeDefinitions);
    }
    [Fact]
    public void RemoveAttributeDefinition_With_Unknown_Id_Should_Do_Nothing()
    {
        var category = Category.Create(
            CategoryName.Create("Mobile"));

        category.RemoveAttributeDefinition(Guid.NewGuid());

        Assert.Empty(category.AttributeDefinitions);
    }
}
