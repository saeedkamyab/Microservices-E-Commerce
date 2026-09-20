using Catalog.API.Endpoints.Categories;

namespace Catalog.API
{
    public static class EndpointRegistrationExtensions
    {
        public static void MapCatalogEndpoints(this WebApplication app)
        {
            app.MapGetCategoriesEndpoint();
            app.MapGetCategoryByIdEndpoint();
            app.MapCreateCategoryEndpoint();
            app.MapActivateCategoryEndpoint();
            app.MapDeactivateCategoryEndpoint();
        }
    }
}
