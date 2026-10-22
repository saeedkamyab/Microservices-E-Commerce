using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Infrastructure.Caching;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Interceptors;
using Catalog.Infrastructure.Persistence.Repositories;
using Catalog.Infrastructure.Persistence.Repositories.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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

        services
        .AddHealthChecks()
        .AddNpgSql(
        configuration.GetConnectionString(
            "CatalogDatabaseConnection")!);

        services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);

            options.AddInterceptors(
                sp.GetRequiredService<CacheInvalidationOutboxInterceptor>());
        });


        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryReadService, CategoryReadService>();

        services.AddScoped<IProductRepository,  ProductRepository>();
        services.AddScoped<IProductReadService, ProductReadService>();



        services.AddScoped<IUnitOfWork> (sp =>
            sp.GetRequiredService<CatalogDbContext>());

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =
                configuration.GetConnectionString("Redis");

            options.InstanceName = "catalog:";
        });


        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var connectionString =
                configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException(
                    "Redis connection string is missing.");

            var options = ConfigurationOptions.Parse(connectionString);

            options.AbortOnConnectFail = false;
            options.ConnectRetry = 2;
            options.ConnectTimeout = 3000;

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<ICacheVersionService, RedisCacheVersionService>();
        services.AddScoped<ICacheInvalidationContext,CacheInvalidationContext>();
        services.AddScoped<CacheInvalidationOutboxInterceptor>();

        return services;
    }
}
