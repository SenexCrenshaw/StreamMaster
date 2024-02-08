using StreamMaster.Domain.Extensions;

namespace StreamMaster.Domain.Cache;

public class Cache<T>(Func<T> valueFetcher, TimeSpan refreshInterval) where T : notnull
{
    private T? _cachedValue = default;
    private DateTime _lastFetchedTime;
    private readonly Func<T> _valueFetcher = valueFetcher ?? throw new ArgumentNullException(nameof(valueFetcher));

    public T GetValue()
    {
        if (_cachedValue is null || SMDT.UtcNow - _lastFetchedTime > refreshInterval)
        {
            _cachedValue = _valueFetcher() ?? throw new InvalidOperationException("Fetched value cannot be null");
            _lastFetchedTime = SMDT.UtcNow;
        }

        return _cachedValue;
    }
}