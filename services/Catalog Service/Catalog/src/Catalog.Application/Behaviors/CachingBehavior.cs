using Catalog.Application.Abstractions.Caching;
using MediatR;

namespace Catalog.Application.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ICacheVersionService versionService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheable)
            return await next(cancellationToken);

        var version = await versionService.GetVersionAsync(
            cacheable.CacheVersionKey,
            cancellationToken);

        var key = $"{cacheable.CacheKey}:v{version}";

        var cached = await cacheService.GetAsync<TResponse>(
            key,
            cancellationToken);

        if (cached is not null)
            return cached;

        var response = await next(cancellationToken);

        var latestVersion = await versionService.GetVersionAsync(
            cacheable.CacheVersionKey,
            cancellationToken);

        if (latestVersion == version)
        {
            await cacheService.SetAsync(
                key,
                response,
                cacheable.CacheExpiration,
                cancellationToken);
        }

        return response;
    }
}
