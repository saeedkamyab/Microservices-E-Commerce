namespace Catalog.Application.Abstractions.Caching;

public interface ICacheableQuery
{
    string CacheKey { get; }
    string CacheVersionKey { get; }
    TimeSpan CacheExpiration { get; }
}
