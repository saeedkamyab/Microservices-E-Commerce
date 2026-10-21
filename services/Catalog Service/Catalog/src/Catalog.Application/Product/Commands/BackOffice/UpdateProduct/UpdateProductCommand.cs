using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Common.Caching;
using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? ProductDescription,
    Guid CategoryId,
    decimal Price,
    IReadOnlyCollection<ProductSpecificationInput> Specifications) : IRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheVersionKeys =>
        [CatalogCacheKeys.backOfficeProductsVersion];
}

public sealed record ProductSpecificationInput(
    Guid AttributeDefinitionId,
    string Value);