using Catalog.Application.Category.Queries.BackOffice.GetCategories;
using Catalog.Application.Category.Queries.BackOffice.GetCategoryById;
using Catalog.Application.Common;
using Catalog.Application.Common.Enums;
using static Catalog.Application.Common.Enums.SortEnum;

namespace Catalog.Application.Abstractions.Persistence.Repositories.Queries;

public interface ICategoryReadService
{
    Task<PagedResult<ReadModels.CategoryReadModel>> GetCategoriesAsync(
       string? search,
       CategoryStatusFilter? status,
       CategorySortBy sortBy,
       SortDirection sortDirection,
       int pageNumber,
       int pageSize,
       CancellationToken cancellationToken);

    Task<CategoryDetailsResult?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken);

}
