using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Common.Caching;
using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.ActivateProduct;

public sealed record ActivateProductCommand(Guid ProductId) : IRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheVersionKeys =>
        [CatalogCacheKeys.backOfficeProductsVersion];

}