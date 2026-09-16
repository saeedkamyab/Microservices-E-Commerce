using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
         this IServiceCollection services,
         IConfiguration configuration)
    {

        var connectionString =
          configuration.GetConnectionString("CatalogDatabaseConnection")
          ?? throw new InvalidOperationException(
              "Connection string 'CatalogDatabaseConnection' was not found.");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString));


        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork> (sp =>
            sp.GetRequiredService<CatalogDbContext>());



        return services;
    }
}
