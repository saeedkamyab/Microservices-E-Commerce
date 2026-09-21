using BackOffice.ApiClient.Catalog.Interfaces;
using BackOffice.Components.Components.Category.Models;
using BackOffice.Components.Enums;
using BackOffice.Contracts.Catalog.Category;
using Microsoft.AspNetCore.Components;

namespace BackOffice.Components.Components.Category;

public partial class CategoryList
{
    [Inject]
    private ICatalogApiClient CatalogApiClient { get; set; } = null!;

    private PagedResponse<CategoryListItemResponse>? Result;

    private string Search { get; set; } = string.Empty;

    private string Status { get; set; } = string.Empty;

    private int PageNumber { get; set; } = 1;

    private const int PageSize = 20;

    private bool IsLoading;

    private string? ErrorMessage;

    private string? SuccessMessage;

    private bool IsCategoryModalVisible;

    private FormMode CurrentFormMode;

    private CategoryFormModel CategoryFormModel { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }

    private void OpenCreateModal()
    {
        CategoryFormModel = new();

        CurrentFormMode = FormMode.Create;

        IsCategoryModalVisible = true;
    }
    private void OpenEditModal(
     CategoryListItemResponse category)
    {
        CategoryFormModel = new()
        {
            Id = category.Id,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId
        };

        CurrentFormMode = FormMode.Edit;

        IsCategoryModalVisible = true;
    }

    private async Task HandleCategorySaved()
    {
        IsCategoryModalVisible = false;

        await LoadCategoriesAsync();
    }

    private async Task SearchAsync()
    {
        SuccessMessage = null;
        PageNumber = 1;

        await LoadCategoriesAsync();
    }

    private async Task PreviousPageAsync()
    {
        if (Result?.HasPreviousPage != true)
            return;

        PageNumber--;

        await LoadCategoriesAsync();
    }

    private async Task NextPageAsync()
    {
        if (Result?.HasNextPage != true)
            return;

        PageNumber++;

        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            Result = await CatalogApiClient.GetCategoriesAsync(
                search: Search,
                status: Status,
                sortBy: "name",
                sortDirection: "asc",
                pageNumber: PageNumber,
                pageSize: PageSize);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ChangeStatusAsync(
     CategoryListItemResponse category,
     bool isActive)
    {

        var result = isActive
            ? await CatalogApiClient.ActivateCategoryAsync(category.Id)
            : await CatalogApiClient.DeactivateCategoryAsync(category.Id);

        if (!result.IsSuccess)
        {
            ErrorMessage =
                result.Error?.Message ?? "Failed to change category status.";

            await LoadCategoriesAsync();
            return;
        }

        await LoadCategoriesAsync();
    }
}