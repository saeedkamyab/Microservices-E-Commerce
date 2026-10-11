using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Common;
using Catalog.Application.Common.Caching;
using Catalog.Application.Common.Enums;
using MediatR;
using static Catalog.Application.Common.Enums.SortEnum;

namespace Catalog.Application.Category.Queries.StoreFront.GetCategories;

public sealed class GetCategoriesQueryHandler
       : IRequestHandler<
        GetCategoriesQuery,
        PagedResult<CategoryListItem>>
{
    private readonly ICategoryReadService _categoryReadService;
    private readonly ICacheVersionService _cacheVersionService;
    private readonly ICacheService _cacheService ;

    private static readonly TimeSpan CacheExpiration =
       TimeSpan.FromMinutes(5);
    public GetCategoriesQueryHandler(
        ICategoryReadService categoryReadService,
        ICacheVersionService cacheVersionService,
        ICacheService cacheService)
    {
        _categoryReadService = categoryReadService;
        _cacheVersionService = cacheVersionService;
        _cacheService = cacheService;
    }
    public async Task<PagedResult<CategoryListItem>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var sortBy = StringToEnumConvertor
          .ToEnum<CategorySortBy>(request.SortBy);

        var sortDirection = StringToEnumConvertor
            .ToEnum<SortDirection>(request.SortDirection);

        var version = await _cacheVersionService.GetVersionAsync(
          CatalogCacheKeys.StoreFrontCategoriesVersion,
          cancellationToken);

        var cacheKey = CreateCacheKey(
          request, sortBy, sortDirection, version);

        var cachedResult = await _cacheService
         .GetAsync<PagedResult<CategoryListItem>>(
             cacheKey,
             cancellationToken);

        if (cachedResult is not null)
            return cachedResult;


        var result = await _categoryReadService.GetCategoriesAsync(
       request.Search,
       CategoryStatusFilter.Active,
       sortBy,
       sortDirection,
       request.PageNumber,
       request.PageSize,
       cancellationToken);


        var items = result.Items.Select(x => new CategoryListItem(
                                        x.Id, x.Name,
                                        x.ParentCategoryId,
                                        x.ParentCategoryName)).ToArray();


        var response = new PagedResult<CategoryListItem>(
          items,
          result.TotalCount,
          result.PageNumber,
          result.PageSize);


        var latestVersion = await _cacheVersionService.GetVersionAsync(
           CatalogCacheKeys.StoreFrontCategoriesVersion,
           cancellationToken);
        if (latestVersion == version)
        {
            await _cacheService.SetAsync(
                cacheKey,
                response,
                CacheExpiration,
                cancellationToken);
        }

        return response;
    }



    private static string CreateCacheKey(
       GetCategoriesQuery request,
       CategorySortBy sortBy,
       SortDirection sortDirection,
       long version)
    {
        var search = request.Search?.Trim().ToLowerInvariant()
            ?? string.Empty;

        var encodedSearch = Convert.ToHexString(
            System.Text.Encoding.UTF8.GetBytes(search));

        return
            $"storefront:categories:v{version}:" +
            $"search={encodedSearch}:" +
            $"sort={sortBy}:" +
            $"direction={sortDirection}:" +
            $"page={request.PageNumber}:" +
            $"size={request.PageSize}";
    }
}
