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

    public async Task<ApiResult> CreateCategoryAsync(CreateCategoryRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                   "api/categories",
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
}
