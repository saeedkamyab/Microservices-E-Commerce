using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Common.Caching;
using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.CreateProduct;


public sealed record CreateProductCommand(
    string Name,
    string? Description,
    Guid CategoryId,
    decimal Price,
IReadOnlyCollection<ProductSpecificationInput> Specifications) : IRequest<Guid>, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheVersionKeys =>
        [CatalogCacheKeys.backOfficeProductsVersion];
}

public sealed record ProductSpecificationInput(
    Guid AttributeDefinitionId,
    string Value);
