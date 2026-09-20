using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Common;
using MediatR;
using static Catalog.Application.Common.SortEnum;

namespace Catalog.Application.Category.Queries.GetCategories;

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
        return _categoryReadService.GetPagedAsync(
            request.Search,
            ParseCategoryStatus(request.Status),
            ParseCategorySortBy(request.SortBy),
            ParseSortDirection(request.SortDirection),
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }




    private static CategoryStatusFilter? ParseCategoryStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return null;

        return status.ToLowerInvariant() switch
        {
            "active" => CategoryStatusFilter.Active,
            "inactive" => CategoryStatusFilter.Inactive,

            _ => throw new ArgumentException(
                $"Invalid category status '{status}'.")
        };
    }
    private static CategorySortBy ParseCategorySortBy(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return CategorySortBy.Name;

        return sortBy.ToLowerInvariant() switch
        {
            "name" => CategorySortBy.Name,
            "status" => CategorySortBy.Status,

            _ => throw new ArgumentException(
                $"Invalid category sort by '{sortBy}'.")
        };
    }
    private static SortDirection ParseSortDirection(string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortDirection))
            return SortDirection.Asc;

        return sortDirection.ToLowerInvariant() switch
        {
            "asc" => SortDirection.Asc,
            "desc" => SortDirection.Desc,

            _ => throw new ArgumentException(
                $"Invalid sort direction '{sortDirection}'.")
        };
    }


}
