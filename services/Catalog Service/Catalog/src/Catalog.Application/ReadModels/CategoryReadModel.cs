namespace Catalog.Application.ReadModels;

public sealed record CategoryReadModel(
  Guid Id,
  string Name,
  string Status,
  Guid? ParentCategoryId,
  string? ParentCategoryName);
