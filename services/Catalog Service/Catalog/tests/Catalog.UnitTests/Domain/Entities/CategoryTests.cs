using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;

namespace Catalog.UnitTests.Domain.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithNullName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Category.Create(null!));
    }

    [Fact]
    public void Create_WithEmptyGuidParent_SetsParentToNullAndInitializesFields()
    {
        var name = CategoryName.Create("Electronics");
        var category = Category.Create(name, Guid.Empty);

        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal(name, category.CategoryName);
        Assert.Null(category.ParentCategoryId);
        Assert.Equal(CategoryStatus.Active, category.Status);
    }

    [Fact]
    public void Rename_WithNull_ThrowsArgumentNullException()
    {
        var category = Category.Create(CategoryName.Create("Books"));

        Assert.Throws<ArgumentNullException>(() => category.Rename(null!));
    }

    [Fact]
    public void Rename_ToSameName_DoesNotChange()
    {
        var name = CategoryName.Create("Toys");
        var category = Category.Create(name);

        category.Rename(name);

        Assert.Equal(name, category.CategoryName);
    }

    [Fact]
    public void Rename_ToDifferentName_UpdatesName()
    {
        var category = Category.Create(CategoryName.Create("Home"));
        var newName = CategoryName.Create("Home & Garden");

        category.Rename(newName);

        Assert.Equal(newName, category.CategoryName);
    }

    [Fact]
    public void ActivateDeactivate_Behavior_WorksAsExpected()
    {
        var category = Category.Create(CategoryName.Create("Garden"));

        // initially Active
        Assert.Equal(CategoryStatus.Active, category.Status);

        category.Deactivate();
        Assert.Equal(CategoryStatus.Inactive, category.Status);

        // Deactivating again should be no-op
        category.Deactivate();
        Assert.Equal(CategoryStatus.Inactive, category.Status);

        category.Activate();
        Assert.Equal(CategoryStatus.Active, category.Status);

        // Activating again should be no-op
        category.Activate();
        Assert.Equal(CategoryStatus.Active, category.Status);
    }

    [Fact]
    public void AddAttributeDefinition_Null_ThrowsArgumentNullException()
    {
        var category = Category.Create(CategoryName.Create("Appliances"));

        Assert.Throws<ArgumentNullException>(() => category.AddAttributeDefinition(null!));
    }

    [Fact]
    public void AddAttributeDefinition_DuplicateName_ThrowsInvalidOperationException()
    {
        var category = Category.Create(CategoryName.Create("Clothing"));

        var def1 = CategoryAttributeDefinition.Create(Name.Create("Color"), AttributeType.Text, false);
        var def2 = CategoryAttributeDefinition.Create(Name.Create("color"), AttributeType.Text, false); // case-insensitive duplicate

        category.AddAttributeDefinition(def1);

        var ex = Assert.Throws<InvalidOperationException>(() => category.AddAttributeDefinition(def2));
        Assert.Equal("An attribute with the same name already exists.", ex.Message);
    }

    [Fact]
    public void AddAttributeDefinition_OptionTypeWithNoOptions_ThrowsInvalidOperationException()
    {
        var category = Category.Create(CategoryName.Create("Shoes"));

        var optionDef = CategoryAttributeDefinition.Create(Name.Create("Size"), AttributeType.Option, false);
        // no options added

        var ex = Assert.Throws<InvalidOperationException>(() => category.AddAttributeDefinition(optionDef));
        Assert.Equal("attribute with option type must have at least one option.", ex.Message);
    }

    [Fact]
    public void AddAttributeDefinition_OptionTypeWithOptions_AddsSuccessfully()
    {
        var category = Category.Create(CategoryName.Create("Accessories"));

        var optionDef = CategoryAttributeDefinition.Create(Name.Create("Material"), AttributeType.Option, false);
        optionDef.AddOption(AttributeOption.Create("Leather"));
        optionDef.AddOption(AttributeOption.Create("Fabric"));

        category.AddAttributeDefinition(optionDef);

        Assert.Single(category.AttributeDefinitions);
        var added = category.AttributeDefinitions.First();
        Assert.Equal(optionDef.Id, added.Id);
        Assert.Equal(AttributeType.Option, added.Type);
        Assert.Equal(2, added.Options.Count);
    }

    [Fact]
    public void RemoveAttributeDefinition_NonExisting_NoOp()
    {
        var category = Category.Create(CategoryName.Create("Stationery"));

        // removing non-existing should not throw and collection remains empty
        category.RemoveAttributeDefinition(Guid.NewGuid());
        Assert.Empty(category.AttributeDefinitions);
    }

    [Fact]
    public void RemoveAttributeDefinition_RemovesExistingDefinition()
    {
        var category = Category.Create(CategoryName.Create("Furniture"));

        var def = CategoryAttributeDefinition.Create(Name.Create("Finish"), AttributeType.Text, false);
        category.AddAttributeDefinition(def);

        Assert.Single(category.AttributeDefinitions);

        category.RemoveAttributeDefinition(def.Id);

        Assert.Empty(category.AttributeDefinitions);
    }
}

