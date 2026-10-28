using Catalog.Application.Abstractions.Caching;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Outbox;
using Catalog.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Catalog.IntegrationTests.Outbox;

public sealed class CacheInvalidationOutboxInterceptorTests(
    PostgreSqlFixture fixture)
    : IClassFixture<PostgreSqlFixture>
{

    [Fact]
    public async Task SaveChangesAsync_WithPendingCacheKey_PersistsOutboxMessage()
    {
        await using var scope = fixture.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CatalogDbContext>();

        var invalidationContext = scope.ServiceProvider
            .GetRequiredService<ICacheInvalidationContext>();

        var cacheKey = $"integration-test:{Guid.NewGuid():N}";

        invalidationContext.AddKeys([cacheKey]);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var messages = await dbContext
            .Set<OutboxMessage>()
            .Where(message => message.Type == "CacheInvalidation")
            .ToListAsync(CancellationToken.None);

        Assert.Contains(
            messages,
            message => message.Payload.Contains(cacheKey));
    }
}