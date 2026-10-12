using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Common;
using Catalog.Application.Common.Caching;
using MediatR;

namespace Catalog.Application.Category.Queries.StoreFront.GetCategories;

public sealed record GetCategoriesQuery(
    string? Search,
    string? SortBy,
    string? SortDirection,
    int PageNumber,
    int PageSize)
    : IRequest<PagedResult<CategoryListItem>>, ICacheableQuery
{
    public string CacheVersionKey =>
       CatalogCacheKeys.StoreFrontCategoriesVersion;

    public TimeSpan CacheExpiration =>
        TimeSpan.FromMinutes(5);

    public string CacheKey =>
        $"storefront:categories:" +
        $"search={Convert.ToHexString(
            System.Text.Encoding.UTF8.GetBytes(
                Search?.Trim().ToLowerInvariant() ?? ""))}:" +
        $"sort={SortBy?.Trim().ToLowerInvariant()}:" +
        $"direction={SortDirection?.Trim().ToLowerInvariant()}:" +
        $"page={PageNumber}:" +
        $"size={PageSize}";
}