using Catalog.Application.Abstractions.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Catalog.Application.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ICacheVersionService versionService,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
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

        long version;
        TResponse? cached;

        try
        {
            version = await versionService.GetVersionAsync(
                cacheable.CacheVersionKey,
                cancellationToken);

            var cacheKey = $"{cacheable.CacheKey}:v{version}";

            cached = await cacheService.GetAsync<TResponse>(
                cacheKey,
                cancellationToken);

            if (cached is not null)
                return cached;
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            logger.LogWarning(
                ex,
                "Cache read failed for {RequestType}. Bypassing cache.",
                typeof(TRequest).Name);

            return await next(cancellationToken);
        }

        // Execute Handler outside the cache exception handler.
        var response = await next(cancellationToken);

        try
        {
            var latestVersion = await versionService.GetVersionAsync(
                cacheable.CacheVersionKey,
                cancellationToken);

            if (latestVersion == version)
            {
                var cacheKey = $"{cacheable.CacheKey}:v{version}";

                await cacheService.SetAsync(
                    cacheKey,
                    response,
                    cacheable.CacheExpiration,
                    cancellationToken);
            }
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            logger.LogWarning(
                ex,
                "Cache write failed for {RequestType}. Returning database result.",
                typeof(TRequest).Name);
        }

        return response;
    }

    private static bool IsCacheFailure(
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return false;

        return exception is RedisException
            or TimeoutException
            or System.IO.IOException;
    }
}