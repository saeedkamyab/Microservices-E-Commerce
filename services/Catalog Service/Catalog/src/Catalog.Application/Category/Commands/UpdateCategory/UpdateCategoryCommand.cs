using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Common.Caching;
using MediatR;

namespace Catalog.Application.Category.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    Guid? ParentCategoryId
    ) : IRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheVersionKeys =>
           [CatalogCacheKeys.StoreFrontCategoriesVersion];
}