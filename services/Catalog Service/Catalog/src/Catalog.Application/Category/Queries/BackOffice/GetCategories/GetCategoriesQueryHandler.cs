using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Common;
using Catalog.Application.Common.Enums;
using MediatR;
using static Catalog.Application.Common.Enums.SortEnum;

namespace Catalog.Application.Category.Queries.BackOffice.GetCategories;

public sealed class GetCategoriesQueryHandler
       : IRequestHandler<
        GetCategoriesQuery,
        PagedResult<CategoryListItem>>
{
    private readonly ICategoryReadService _categoryReadService;

    public GetCategoriesQueryHandler(
        ICategoryReadService categoryReadService)
    {
        _categoryReadService = categoryReadService;
    }

    public Task<PagedResult<CategoryListItem>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return _categoryReadService.GetCategoriesAsync(
            request.Search,
            StringToEnumConvertor.ToNullableEnum<CategoryStatusFilter>(request.Status),
            StringToEnumConvertor.ToEnum<CategorySortBy>(request.SortBy),
            StringToEnumConvertor.ToEnum<SortDirection>(request.SortDirection),
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
