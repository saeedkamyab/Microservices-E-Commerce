namespace BackOffice.Contracts.Catalog.Category;

public sealed class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;

    public Guid? ParentCategoryId { get; set; }

    public List<CategoryAttributeRequest> Attributes { get; set; } = [];
}
