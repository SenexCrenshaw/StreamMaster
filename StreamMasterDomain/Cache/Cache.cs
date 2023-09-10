namespace StreamMasterDomain.Cache;

public class Cache<T> where T : notnull
{
    private T? _cachedValue;
    private DateTime _lastFetchedTime;
    private readonly Func<T> _valueFetcher;
    private readonly TimeSpan _refreshInterval;

    public Cache(Func<T> valueFetcher, TimeSpan refreshInterval)
    {
        _valueFetcher = valueFetcher ?? throw new ArgumentNullException(nameof(valueFetcher));
        _refreshInterval = refreshInterval;
        _cachedValue = default;
    }

    public T GetValue()
    {
        if (_cachedValue is null || (DateTime.UtcNow - _lastFetchedTime) > _refreshInterval)
        {
            _cachedValue = _valueFetcher() ?? throw new InvalidOperationException("Fetched value cannot be null");
            _lastFetchedTime = DateTime.UtcNow;
        }

        return _cachedValue;
    }
}