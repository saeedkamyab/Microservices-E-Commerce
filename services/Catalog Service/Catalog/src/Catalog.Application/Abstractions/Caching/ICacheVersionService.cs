namespace Catalog.Application.Abstractions.Caching;

public interface ICacheVersionService
{
    Task<long> GetVersionAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task<long> IncrementVersionAsync(
        string key,
        CancellationToken cancellationToken = default);
}
