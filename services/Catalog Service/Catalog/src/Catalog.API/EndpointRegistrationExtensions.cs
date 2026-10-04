using Catalog.API.Endpoints.Categories.BackOffice;
using Catalog.API.Endpoints.Products.BackOffice;

namespace Catalog.API
{
    public static class EndpointRegistrationExtensions
    {
        public static void MapCatalogEndpoints(this WebApplication app)
        {

            //Categories
            app.MapGetCategoriesEndpoint();
            app.MapGetCategoryByIdEndpoint();
            app.MapCreateCategoryEndpoint();
            app.MapUpdateCategoryEndpoint();
            app.MapActivateCategoryEndpoint();
            app.MapDeactivateCategoryEndpoint();
            app.MapAddCategoryAttributeDefinitionEndpoint();
            app.MapRemoveCategoryAttributeDefinitionEndpoint(); 

            //Products
            app.MapGetProductsEndpoint();
            app.MapGetProductByIdEndpoint();
            app.MapCreateProductEndpoint();
            app.MapUpdateProductEndpoint();
            app.MapActivateProductEndpoint();
            app.MapDeactivateProductEndpoint();

        }
    }
}
