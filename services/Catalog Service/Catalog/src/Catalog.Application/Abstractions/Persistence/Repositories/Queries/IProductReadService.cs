using Catalog.Application.Common;
using static Catalog.Application.Common.Enums.SortEnum;
using Catalog.Application.Common.Enums;
using Catalog.Application.Product.Queries.BackOffice.GetProductById;
using Catalog.Application.Product.Queries.BackOffice.GetProducts;

namespace Catalog.Application.Abstractions.Persistence.Repositories.Queries;

public interface IProductReadService
{
    Task<PagedResult<ProductListItem>> GetProductsAsync(
      string? search,
      ProductStatusFilter? status,
      ProductSortBy sortBy,
      SortDirection sortDirection,
      int pageNumber,
      int pageSize,
      CancellationToken cancellationToken);

    Task<ProductDetailsResult?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken);
}
