using Catalog.Domain.Enums;

namespace Catalog.Application.Category.Queries.GetCategoryById;

public sealed record CategoryDetailsResult(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    string Status,
    IReadOnlyCollection<CategoryAttributeResult> Attributes);

public sealed record CategoryAttributeResult(
    Guid Id,
    string Name,
    AttributeType Type,
    bool IsRequired,
    IReadOnlyCollection<string> Options);
