using Catalog.Application.Category.Queries.GetCategories;
using Catalog.Application.Common;
using static Catalog.Application.Common.SortEnum;

namespace Catalog.Application.Abstractions.Persistence.Repositories.Queries;

public interface ICategoryReadService
{
    Task<PagedResult<CategoryListItem>> GetPagedAsync(
       string? search,
       CategoryStatusFilter? status,
       CategorySortBy sortBy,
       SortDirection sortDirection,
       int pageNumber,
       int pageSize,
       CancellationToken cancellationToken);
}
