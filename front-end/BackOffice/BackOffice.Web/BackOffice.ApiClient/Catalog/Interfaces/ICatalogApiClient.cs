using BackOffice.ApiClient.Common;
using BackOffice.Contracts.Catalog.Category;

namespace BackOffice.ApiClient.Catalog.Interfaces;

public interface ICatalogApiClient
{

    Task<PagedResponse<CategoryListItemResponse>> GetCategoriesAsync(
    string? search = null,
    string? status = null,
    string? sortBy = null,
    string? sortDirection = null,
    int pageNumber = 1,
    int pageSize = 20,
    CancellationToken cancellationToken = default);

    Task<ApiResult> CreateCategoryAsync(
      CreateCategoryRequest request,
      CancellationToken cancellationToken = default);

    Task<ApiResult> ActivateCategoryAsync(
    Guid categoryId,
    CancellationToken cancellationToken = default);

    Task<ApiResult> DeactivateCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);

}
