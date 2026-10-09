using Catalog.API.Endpoints.StoreFront.Categories;
using Catalog.API.Endpoints.StoreFront.Products;

namespace Catalog.API.Endpoints.StoreFront
{
    public static class StoreFrontEndpointRegistration
    {
        public static void MapStoreFrontEndpoints(this WebApplication app)
        {


            var StoreFrontGroup = app.MapGroup("/api/storefront");


  
            //Categories 
            StoreFrontGroup.MapGetCategoriesEndpoint();
            StoreFrontGroup.MapGetCategoryByIdEndpoint();


            //Products 
            StoreFrontGroup.MapGetProductsEndpoint();
            StoreFrontGroup.MapGetProductByIdEndpoint();

     

       
        
        }
    }
}
