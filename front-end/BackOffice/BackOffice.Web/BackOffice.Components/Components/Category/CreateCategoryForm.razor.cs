using BackOffice.ApiClient.Catalog.Interfaces;
using BackOffice.Contracts.Catalog.Category;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace BackOffice.Components.Components.Category;

public partial class CreateCategoryForm
{
    [Inject]
    private ICatalogApiClient CatalogApiClient { get; set; } = null!;

    [Parameter]
    public EventCallback OnCreated { get; set; }

    private CreateCategoryRequest Model { get; set; } = new();

    private CategoryAttributeRequest CategoryAttribute { get; set; } = new();

    private bool IsSubmitting;

    private string? ErrorMessage;

    private void AddAttribute()
    {
        Model.Attributes.Add(CategoryAttribute);
        CategoryAttribute = new();
    }

    private void RemoveAttribute(CategoryAttributeRequest attribute)
    {
        Model.Attributes.Remove(attribute);
    }

    private async Task CreateCategory()
    {
        try
        {
            IsSubmitting = true;
            ErrorMessage = null;

            var result =
                await CatalogApiClient.CreateCategoryAsync(Model);

            if (!result.IsSuccess)
            {
                ErrorMessage =
                    result.Error?.Message
                    ?? "An unexpected error occurred.";

                return;
            }

            Model = new CreateCategoryRequest();

            await OnCreated.InvokeAsync();
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