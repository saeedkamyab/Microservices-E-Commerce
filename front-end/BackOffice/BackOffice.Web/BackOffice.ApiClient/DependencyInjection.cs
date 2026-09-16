using BackOffice.ApiClient.Catalog;
using BackOffice.ApiClient.Catalog.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BackOffice.ApiClient
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBackOfficeApiClients(
       this IServiceCollection services,
       string catalogBaseAddress)
        {
            services.AddHttpClient<ICatalogApiClient, CatalogApiClient>(
                client =>
                {
                    client.BaseAddress =
                        new Uri(catalogBaseAddress);
                });

            return services;
        }
    }
}
