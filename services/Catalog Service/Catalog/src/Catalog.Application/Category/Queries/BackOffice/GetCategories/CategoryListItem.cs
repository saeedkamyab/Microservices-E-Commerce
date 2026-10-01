namespace Catalog.Application.Category.Queries.BackOffice.GetCategories;

public sealed record CategoryListItem(
  Guid Id,
  string Name,
  string Status,
  Guid? ParentCategoryId,
  string? ParentCategoryName);
