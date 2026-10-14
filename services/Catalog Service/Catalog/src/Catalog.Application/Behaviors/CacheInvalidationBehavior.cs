using Catalog.Application.Abstractions.Caching;
using MediatR;

namespace Catalog.Application.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>(
      ICacheInvalidationContext invalidationContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IInvalidatesCache invalidation)
            return await next(cancellationToken);

        invalidationContext.AddKeys(
            invalidation.CacheVersionKeys);

        try
        {
            return await next(cancellationToken);
        }
        finally
        {
            invalidationContext.Clear();
        }
    }
}

