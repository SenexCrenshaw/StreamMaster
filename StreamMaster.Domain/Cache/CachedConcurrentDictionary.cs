using Microsoft.Extensions.Caching.Memory;

using System.Collections.Concurrent;

namespace StreamMaster.Domain.Cache;

public class CachedConcurrentDictionary<TKey, TValue>(IMemoryCache memoryCache, string cacheKey, TimeSpan? cacheExpiration = null) where TKey : notnull
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly string _cacheKey = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));

    private ConcurrentDictionary<TKey, TValue> Dictionary => _memoryCache.GetOrCreate(_cacheKey, entry =>
    {
        if (cacheExpiration.HasValue)
        {
            entry.SlidingExpiration = cacheExpiration.Value;
        }
        return new ConcurrentDictionary<TKey, TValue>();
    })!;

    public bool TryGetValue(TKey key, out TValue? value)
    {
        return Dictionary.TryGetValue(key, out value);
    }

    public bool TryAdd(TKey key, TValue value)
    {
        return Dictionary.TryAdd(key, value);
    }

    public bool TryRemove(TKey key, out TValue? value)
    {
        return Dictionary.TryRemove(key, out value);
    }

    public bool TryRemove(TKey key)
    {
        return Dictionary.TryRemove(key, out _);
    }

    public IEnumerable<TValue> Values => Dictionary.Values;

    public bool ContainsKey(TKey key)
    {
        return Dictionary.ContainsKey(key);
    }

    public void Clear()
    {
        Dictionary.Clear();
    }

    public TValue? GetValueOrDefault(TKey key, TValue? defaultValue = default)
    {
        return Dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
    }

    public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
    {
        return Dictionary.AddOrUpdate(key, addValue, updateValueFactory);
    }
    public TValue AddOrUpdate(TKey key, TValue addValue)
    {
        return Dictionary.AddOrUpdate(key, addValue, (_, _) => addValue);
    }
    public TValue IncrementValue(TKey key, TValue incrementValue, TValue initialValue)
    {
        return Dictionary.AddOrUpdate(key, initialValue, (_, oldValue) => (dynamic?)oldValue + incrementValue);
    }

    public TValue DecrementValue(TKey key, TValue decrementValue, TValue initialValue)
    {
        return Dictionary.AddOrUpdate(key, initialValue, (_, oldValue) => (dynamic?)oldValue - decrementValue);
    }
}