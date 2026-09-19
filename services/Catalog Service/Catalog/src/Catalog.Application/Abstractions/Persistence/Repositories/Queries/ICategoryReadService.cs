using Catalog.Application.Category.Queries.GetCategories;
using Catalog.Application.Common;

namespace Catalog.Application.Abstractions.Persistence.Repositories.Queries;

public interface ICategoryReadService
{
    Task<PagedResult<CategoryListItem>> GetPagedAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
