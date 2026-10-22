using Catalog.Application.Abstractions.Caching;
using Catalog.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace Catalog.Infrastructure.Persistence.Interceptors;

public sealed class CacheInvalidationOutboxInterceptor(
    ICacheInvalidationContext invalidationContext)
    : SaveChangesInterceptor
{
    private readonly HashSet<string> _keysAdded = [];

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AddOutboxMessages(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddOutboxMessages(eventData.Context);

        return base.SavingChangesAsync(
            eventData, result, cancellationToken);
    }

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        Complete();
        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        Complete();

        return base.SavedChangesAsync(
            eventData, result, cancellationToken);
    }

    private void AddOutboxMessages(DbContext? context)
    {
        if (context is not CatalogDbContext dbContext)
            return;

        foreach (var key in invalidationContext.PendingKeys)
        {
            if (_keysAdded.Contains(key))
                continue;

            var message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "CacheInvalidation",
                Payload = JsonSerializer.Serialize(
                    new { CacheVersionKey = key }),
                CreatedAtUtc = DateTime.UtcNow
            };

            dbContext.Set<OutboxMessage>().Add(message);

            _keysAdded.Add(key);
        }
    }

    private void Complete()
    {
        invalidationContext.MarkAsSaved(_keysAdded);
        _keysAdded.Clear();
    }
}