using BackOffice.ApiClient.Catalog.Interfaces;
using BackOffice.ApiClient.Common;
using BackOffice.Contracts.Catalog.Category;
using System.Net.Http.Json;

namespace BackOffice.ApiClient.Catalog;

internal class CatalogApiClient : ICatalogApiClient
{
    private readonly HttpClient _httpClient;

    public CatalogApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResponse<CategoryListItemResponse>>
    GetCategoriesAsync(
        string? search = null,
        string? status = null,
        string? sortBy = null,
        string? sortDirection = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
    {
        $"pageNumber={pageNumber}",
        $"pageSize={pageSize}"
    };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add(
                $"search={Uri.EscapeDataString(search)}");
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query.Add(
                $"status={Uri.EscapeDataString(status)}");
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query.Add(
                $"sortBy={Uri.EscapeDataString(sortBy)}");
        }

        if (!string.IsNullOrWhiteSpace(sortDirection))
        {
            query.Add(
                $"sortDirection={Uri.EscapeDataString(sortDirection)}");
        }

        var url = $"api/catalog/categories?{string.Join("&", query)}";

        var response = await _httpClient.GetAsync(
            url,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(error)
                    ? "Failed to load categories."
                    : error);
        }

        var result =
            await response.Content.ReadFromJsonAsync<
                PagedResponse<CategoryListItemResponse>>(
                    cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The category response was empty.");
    }

    public async Task<ApiResult> CreateCategoryAsync(CreateUpdateCategoryRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                   "api/catalog/categories",
                   request,
                   cancellationToken);

            if (response.IsSuccessStatusCode)
                return ApiResult.Success();

            return await ErrorCreator.CreateErrorResultAsync(
                response,
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            return ApiResult.Failure(
                new ApiError(
                    "connection_error",
                    "Could not connect to Catalog service."));
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult.Failure(
                new ApiError(
                    "timeout",
                    "Catalog service did not respond in time."));
        }
    }


    public async Task<ApiResult> UpdateCategoryAsync(
    Guid categoryId,
    CreateUpdateCategoryRequest request,
    CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/catalog/categories/{categoryId}",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
                return ApiResult.Success();

            return await ErrorCreator.CreateErrorResultAsync(
                response,
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            return ApiResult.Failure(
                new ApiError(
                    "connection_error",
                    "Could not connect to Catalog service."));
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult.Failure(
                new ApiError(
                    "timeout",
                    "Catalog service did not respond in time."));
        }
    }


    public async Task<ApiResult> ActivateCategoryAsync(
      Guid categoryId,
      CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"api/catalog/categories/{categoryId}/activate",
                null,
                cancellationToken);

            if (response.IsSuccessStatusCode)
                return ApiResult.Success();

            return await ErrorCreator.CreateErrorResultAsync(
                response,
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            return ApiResult.Failure(
                new ApiError(
                    "connection_error",
                    "Could not connect to Catalog service."));
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult.Failure(
                new ApiError(
                    "timeout",
                    "Catalog service did not respond in time."));
        }
    }

    public async Task<ApiResult> DeactivateCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"api/catalog/categories/{categoryId}/deactivate",
                null,
                cancellationToken);

            if (response.IsSuccessStatusCode)
                return ApiResult.Success();

            return await ErrorCreator.CreateErrorResultAsync(
                response,
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            return ApiResult.Failure(
                new ApiError(
                    "connection_error",
                    "Could not connect to Catalog service."));
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult.Failure(
                new ApiError(
                    "timeout",
                    "Catalog service did not respond in time."));
        }
    }
}
