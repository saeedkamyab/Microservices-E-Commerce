using BackOffice.ApiClient.Common;
using BackOffice.Contracts.Catalog.Category;

namespace BackOffice.ApiClient.Catalog.Interfaces;

public interface ICatalogApiClient
{
    Task<ApiResult> CreateCategoryAsync(
      CreateCategoryRequest request,
      CancellationToken cancellationToken = default);
}
