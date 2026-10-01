namespace Catalog.Application.Category.Queries.BackOffice.GetCategoryById;

public sealed record CategoryDetailsResult(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    string Status,
    IReadOnlyCollection<CategoryAttributeResult> Attributes);

public sealed record CategoryAttributeResult(
    Guid Id,
    string Name,
    string Type,
    bool IsRequired,
    IReadOnlyCollection<string> Options);
