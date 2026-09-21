using BackOffice.ApiClient.Catalog.Interfaces;
using BackOffice.ApiClient.Common;
using BackOffice.Components.Components.Category.Models;
using BackOffice.Components.Enums;
using BackOffice.Contracts.Catalog.Category;
using Microsoft.AspNetCore.Components;

namespace BackOffice.Components.Components.Category;

public partial class CategoryForm
{
    [Inject]
    private ICatalogApiClient CatalogApiClient { get; set; } = null!;

    [Parameter]
    public FormMode Mode { get; set; }

    [Parameter]
    public EventCallback OnSaved { get; set; }

    [Parameter]
    public CategoryFormModel Model { get; set; }

    private bool IsSubmitting;

    private string? ErrorMessage;


    private async Task SaveAsync()
    {
        try
        {
            IsSubmitting = true;
            ErrorMessage = null;

            var request = new CreateUpdateCategoryRequest
            {
                Name = Model.Name,
                ParentCategoryId = Model.ParentCategoryId
            };

            ApiResult result;

            if (Mode == FormMode.Create)
            {
                result = await CatalogApiClient
                    .CreateCategoryAsync(request);
            }
            else
            {
                if (!Model.Id.HasValue)
                {
                    throw new InvalidOperationException(
                        "CategoryId is required in edit mode.");
                }

                result = await CatalogApiClient
                    .UpdateCategoryAsync(
                        Model.Id.Value,
                        request);
            }

            if (!result.IsSuccess)
            {
                ErrorMessage =
                    result.Error?.Message
                    ?? "An unexpected error occurred.";

                return;
            }

            await OnSaved.InvokeAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred.";
        }
        finally
        {
            IsSubmitting = false;
        }
    }
}