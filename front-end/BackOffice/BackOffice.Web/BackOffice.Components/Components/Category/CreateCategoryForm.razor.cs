using BackOffice.ApiClient.Catalog.Interfaces;
using BackOffice.Contracts.Catalog.Category;
using Microsoft.AspNetCore.Components;

namespace BackOffice.Components.Components.Category
{
    public partial class CreateCategoryForm
    {
        [Inject] private ICatalogApiClient CatalogApiClient { get; set; } = null!;
        private string? SuccessMessage;
        private string? ErrorMessage;

        private CreateCategoryRequest Model { get; set; } = new();

        private bool IsSubmitting;


        private void AddAttribute()
        {
            Model.Attributes.Add(new CategoryAttributeRequest());
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
                SuccessMessage = null;
                ErrorMessage = null;

                var result = await CatalogApiClient.CreateCategoryAsync(Model);

                if (!result.IsSuccess)
                {
                    ErrorMessage =
                        result.Error?.Message
                        ?? "An unexpected error occurred.";

                    return;
                }

                SuccessMessage =
                    "Category created successfully.";

                Model = new CreateCategoryRequest();

            }
            catch (Exception)
            {
                ErrorMessage =
                    "An unexpected error occurred.";
            }
            finally
            {
                IsSubmitting = false;
            }
        }


    }
}