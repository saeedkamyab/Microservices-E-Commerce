namespace BackOffice.Contracts.Catalog.Category;

public sealed class CategoryListItemResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public Guid? ParentCategoryId { get; init; }

    public string? ParentCategoryName { get; init; }
}
