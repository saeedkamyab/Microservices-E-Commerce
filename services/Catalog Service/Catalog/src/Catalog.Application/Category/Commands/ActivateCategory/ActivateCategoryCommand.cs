using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Common.Caching;
using MediatR;

namespace Catalog.Application.Category.Commands.ActivateCategory;

public sealed record ActivateCategoryCommand(Guid CategoryId) : IRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheVersionKeys =>
           [CatalogCacheKeys.StoreFrontCategoriesVersion];
}