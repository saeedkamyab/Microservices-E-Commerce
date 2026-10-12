namespace Catalog.Application.Abstractions.Caching;

public interface IInvalidatesCache
{
    IReadOnlyCollection<string> CacheVersionKeys { get; }
}
