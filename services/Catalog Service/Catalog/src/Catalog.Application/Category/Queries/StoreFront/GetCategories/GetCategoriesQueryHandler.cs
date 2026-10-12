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


    public GetCategoriesQueryHandler(
        ICategoryReadService categoryReadService,
        ICacheVersionService cacheVersionService,
        ICacheService cacheService)
    {
        _categoryReadService = categoryReadService;
       
    }
    public async Task<PagedResult<CategoryListItem>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var sortBy = StringToEnumConvertor
          .ToEnum<CategorySortBy>(request.SortBy);

        var sortDirection = StringToEnumConvertor
            .ToEnum<SortDirection>(request.SortDirection);


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


        return new PagedResult<CategoryListItem>(
          items,
          result.TotalCount,
          result.PageNumber,
          result.PageSize); ;
    }

}
