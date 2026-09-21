namespace BackOffice.Contracts.Catalog.Category;

public sealed class CreateUpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;

    public Guid? ParentCategoryId { get; set; }
}
