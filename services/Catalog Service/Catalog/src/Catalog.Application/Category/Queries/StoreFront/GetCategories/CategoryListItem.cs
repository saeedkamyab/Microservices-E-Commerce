namespace Catalog.Application.Category.Queries.StoreFront.GetCategories;

public sealed record CategoryListItem(
  Guid Id,
  string Name,
  Guid? ParentCategoryId,
  string? ParentCategoryName);
