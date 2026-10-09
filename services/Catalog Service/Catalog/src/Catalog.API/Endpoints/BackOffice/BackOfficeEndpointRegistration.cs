using Catalog.API.Endpoints.BackOffice.Categories;
using Catalog.API.Endpoints.BackOffice.Products;

namespace Catalog.API.Endpoints.BackOffice
{
    public static class BackOfficeEndpointRegistration
    {
        public static void MapBackOfficeEndpoints(this WebApplication app)
        {

            var backOfficeGroup = app.MapGroup("/api/backoffice");
            //        var backOfficeApi = app
            //.MapGroup("/api/backoffice")
            //.RequireAuthorization("CatalogManagement");

            //Categories 
            backOfficeGroup.MapGetCategoriesEndpoint();
            backOfficeGroup.MapGetCategoryByIdEndpoint();
            backOfficeGroup.MapCreateCategoryEndpoint();
            backOfficeGroup.MapUpdateCategoryEndpoint();
            backOfficeGroup.MapActivateCategoryEndpoint();
            backOfficeGroup.MapDeactivateCategoryEndpoint();
            backOfficeGroup.MapAddCategoryAttributeDefinitionEndpoint();
            backOfficeGroup.MapRemoveCategoryAttributeDefinitionEndpoint();

            //Products 
            backOfficeGroup.MapGetProductsEndpoint();
            backOfficeGroup.MapGetProductByIdEndpoint();
            backOfficeGroup.MapCreateProductEndpoint();
            backOfficeGroup.MapUpdateProductEndpoint();
            backOfficeGroup.MapActivateProductEndpoint();
            backOfficeGroup.MapDeactivateProductEndpoint();
            
    
       
        
        }
    }
}
