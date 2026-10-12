using Catalog.Application.Abstractions.Caching;
using MediatR;

namespace Catalog.Application.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>(
    ICacheVersionService versionService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        if (request is IInvalidatesCache invalidation)
        {
            foreach (var key in invalidation.CacheVersionKeys.Distinct())
            {
                await versionService.IncrementVersionAsync(
                    key,
                    cancellationToken);
            }
        }

        return response;
    }
}
