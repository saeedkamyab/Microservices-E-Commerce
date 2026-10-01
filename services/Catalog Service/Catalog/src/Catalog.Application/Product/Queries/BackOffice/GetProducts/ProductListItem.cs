namespace Catalog.Application.Product.Queries.BackOffice.GetProducts;

public sealed record ProductListItem(
  Guid Id,
  string Name,
  string? ProductDescription,
  Guid CategoryId,
  string Status,
  decimal Price,
  string CategoryName);
