namespace Catalog.Application.Product.Queries.BackOffice.GetProductById;

public sealed record ProductDetailsResult(
    Guid Id,
    string Name,
    string? ProductDescription,
    Guid CategoryId,
    decimal Price,
    string Status,
    IReadOnlyCollection<ProductSpecificationResult> Specifications);

public sealed record ProductSpecificationResult(
    Guid AttributeDefinitionId,
    string Value);
