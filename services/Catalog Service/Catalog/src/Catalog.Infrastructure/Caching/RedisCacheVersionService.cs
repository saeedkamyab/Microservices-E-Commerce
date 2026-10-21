using Catalog.Application.Abstractions.Caching;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Caching;

public sealed class RedisCacheVersionService(
    IConnectionMultiplexer redis) : ICacheVersionService
{
    public async Task<long> GetVersionAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var database = redis.GetDatabase();

        var value = await database.StringGetAsync(key);

        cancellationToken.ThrowIfCancellationRequested();

        return value.HasValue ? (long)value : 0;
    }
    public async Task<long> IncrementVersionAsync(
      string key,
      CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var database = redis.GetDatabase();

        var version = await database.StringIncrementAsync(key);

        cancellationToken.ThrowIfCancellationRequested();

        return version;
    }
}
