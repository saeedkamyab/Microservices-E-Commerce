namespace Catalog.Application.Abstractions.Caching;

public interface ICacheInvalidationContext
{
    IReadOnlyCollection<string> PendingKeys { get; }

    void AddKeys(IEnumerable<string> keys);

    void MarkAsSaved(IEnumerable<string> keys);

    void Clear();
}
