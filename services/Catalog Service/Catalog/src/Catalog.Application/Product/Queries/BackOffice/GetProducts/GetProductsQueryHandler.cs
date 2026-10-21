using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Common;
using Catalog.Application.Common.Enums;
using MediatR;
using static Catalog.Application.Common.Enums.SortEnum;

namespace Catalog.Application.Product.Queries.BackOffice.GetProducts;

public sealed class GetProductsQueryHandler
       : IRequestHandler<
        GetProductsQuery,
        PagedResult<ProductListItem>>
{
    private readonly IProductReadService _productReadService;
    private readonly ICacheService _cacheService;
    private static readonly TimeSpan CacheExpiration =
       TimeSpan.FromMinutes(5);

    public GetProductsQueryHandler(
        IProductReadService productReadService,
        ICacheService cacheService)
    {
        _productReadService = productReadService;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<ProductListItem>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var status = StringToEnumConvertor
            .ToNullableEnum<ProductStatusFilter>(request.Status);


        var sortBy = StringToEnumConvertor
            .ToEnum<ProductSortBy>(request.SortBy);

        var sortDirection = StringToEnumConvertor
            .ToEnum<SortDirection>(request.SortDirection);
        
        var cacheKey = CreateCacheKey(
           request, status, sortBy, sortDirection);

        var cachedResult = await _cacheService
            .GetAsync<PagedResult<ProductListItem>>(
                cacheKey,
                cancellationToken);

        if (cachedResult is not null)
            return cachedResult;

        var result =await _productReadService.GetProductsAsync(
            request.Search,
            status,
            sortBy,
            sortDirection,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        await _cacheService.SetAsync(
          cacheKey,
          result,
          CacheExpiration,
          cancellationToken);

        return result;
    }

    private static string CreateCacheKey(
       GetProductsQuery request,
       ProductStatusFilter? status,
       ProductSortBy sortBy,
       SortDirection sortDirection)
    {
        var search = request.Search?.Trim().ToLowerInvariant()
            ?? string.Empty;

     
        var encodedSearch = Convert.ToHexString(
            System.Text.Encoding.UTF8.GetBytes(search));

        return
            $"backoffice:products:list:" +
            $"search={encodedSearch}:" +
            $"status={status}:" +
            $"sort={sortBy}:" +
            $"direction={sortDirection}:" +
            $"page={request.PageNumber}:" +
            $"size={request.PageSize}";
    }


}
