namespace Catalog.Application.Category.Queries.GetCategories;

public sealed record CategoryListItem(
  Guid Id,
  string Name,
  string Status,
  Guid? ParentCategoryId,
  string? ParentCategoryName);
