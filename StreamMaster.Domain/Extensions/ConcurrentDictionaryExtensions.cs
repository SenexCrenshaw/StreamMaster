using System.Collections.Concurrent;

namespace StreamMaster.Domain.Extensions;

public static class ConcurrentDictionaryExtensions
{
    public static TValue FindOrCreate<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> dictionary,
      TKey key,
      Func<TKey, TValue> createValue)
      where TKey : notnull
      where TValue : class
    {
        return dictionary.GetOrAdd(key, createValue);
    }

    public static (TValue value, bool created) FindOrCreateWithStatus<TKey, TValue>(
    this ConcurrentDictionary<TKey, TValue> dictionary,
    TKey key,
    Func<TKey, TValue> createValue)
    where TKey : notnull
    where TValue : class
    {
        bool created = false;


        TValue value = dictionary.GetOrAdd(key, k =>
        {
            created = true;
            return createValue(k);
        });

        return (value, created);
    }
}
