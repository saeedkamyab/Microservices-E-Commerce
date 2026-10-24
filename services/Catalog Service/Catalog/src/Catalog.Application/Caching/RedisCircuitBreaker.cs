namespace Catalog.Application.Caching;

public sealed class RedisCircuitBreaker
{
    private static readonly TimeSpan BreakDuration =
        TimeSpan.FromSeconds(15);

    private readonly object _lock = new();
    private DateTimeOffset _openUntil = DateTimeOffset.MinValue;

    public bool IsOpen
    {
        get
        {
            lock (_lock)
            {
                return DateTimeOffset.UtcNow < _openUntil;
            }
        }
    }

    public void RecordFailure()
    {
        lock (_lock)
        {
            _openUntil = DateTimeOffset.UtcNow + BreakDuration;
        }
    }
}
