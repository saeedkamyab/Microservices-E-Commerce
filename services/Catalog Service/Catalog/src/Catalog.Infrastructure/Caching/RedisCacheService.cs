using Catalog.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Catalog.Infrastructure.Caching;

internal sealed class RedisCacheService(
    IDistributedCache cache) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var json = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (json is null)
            return default;

        return JsonSerializer.Deserialize<T>(
            json,
            JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(
            value,
            JsonOptions);

        await cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            },
            cancellationToken);
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(
            key,
            cancellationToken);
    }
}