using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Common;
using Catalog.Application.Common.Caching;
using MediatR;
using System.Net.NetworkInformation;

namespace Catalog.Application.Product.Queries.BackOffice.GetProducts;

public sealed record GetProductsQuery(
    string? Search,
    string? Status,
    string? SortBy,
    string? SortDirection,
    int PageNumber,
    int PageSize)
    : IRequest<PagedResult<ProductListItem>>, ICacheableQuery
{
    public string CacheVersionKey =>
         CatalogCacheKeys.backOfficeProductsVersion;

    public TimeSpan CacheExpiration =>
        TimeSpan.FromMinutes(5);

    public string CacheKey =>
          $"backoffice:products:list:" +
            $"search={Convert.ToHexString(
            System.Text.Encoding.UTF8.GetBytes(
                Search?.Trim().ToLowerInvariant() ?? ""))}:" +
            $"status={Status}:" +
            $"sort={SortBy?.Trim().ToLowerInvariant()}:" +
            $"direction={SortDirection?.Trim().ToLowerInvariant()}:" +
            $"page={PageNumber}:" +
            $"size={PageSize}";
}