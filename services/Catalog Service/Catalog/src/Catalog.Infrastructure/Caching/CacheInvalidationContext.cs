using Catalog.Application.Abstractions.Caching;

namespace Catalog.Infrastructure.Caching;

public sealed class CacheInvalidationContext
    : ICacheInvalidationContext
{
    private readonly HashSet<string> _pendingKeys = [];

    public IReadOnlyCollection<string> PendingKeys =>
        _pendingKeys.ToArray();

    public void AddKeys(IEnumerable<string> keys)
    {
        foreach (var key in keys)
            _pendingKeys.Add(key);
    }
    public void MarkAsSaved(IEnumerable<string> keys)
    {
        foreach (var key in keys)
            _pendingKeys.Remove(key);
    }

    public void Clear()
    {
        _pendingKeys.Clear();
    }
}
