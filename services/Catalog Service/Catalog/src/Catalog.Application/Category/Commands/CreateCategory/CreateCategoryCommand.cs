using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Common.Caching;
using Catalog.Domain.Enums;
using MediatR;
using System.Net;

namespace Catalog.Application.Category.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId
    ) : IRequest<Guid>, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheVersionKeys =>
         [CatalogCacheKeys.StoreFrontCategoriesVersion];
}