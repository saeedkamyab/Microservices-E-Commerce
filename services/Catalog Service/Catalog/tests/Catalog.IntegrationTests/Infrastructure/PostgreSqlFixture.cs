using Catalog.Application.Abstractions.Caching;
using Catalog.Infrastructure.Caching;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Catalog.IntegrationTests.Infrastructure;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:18-alpine")
            .WithDatabase("catalog_integration_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public IServiceProvider Services { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        var services = new ServiceCollection();

        services.AddScoped<ICacheInvalidationContext, CacheInvalidationContext>();
        services.AddScoped<CacheInvalidationOutboxInterceptor>();

        services.AddDbContext<CatalogDbContext>(
            (serviceProvider, options) =>
            {
                options.UseNpgsql(_postgres.GetConnectionString());

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<
                        CacheInvalidationOutboxInterceptor>());
            });

        Services = services.BuildServiceProvider();

        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CatalogDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (Services is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else if (Services is IDisposable disposable)
            disposable.Dispose();

        await _postgres.DisposeAsync();
    }
}