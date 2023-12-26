using System.Collections.Concurrent;

namespace StreamMaster.Domain.Extensions;

public static class ConcurrentDictionaryExtensions
{
    public static TValue FindOrCreate<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> createValue)
        where TValue : class
    {
        // Try to get the value from the dictionary
        if (!dictionary.TryGetValue(key, out TValue? value))
        {
            // Value not found, create a new value
            value = createValue(key);

            // Attempt to add the new value to the dictionary
            // If another thread has added the value in the meantime, use that value instead
            dictionary.TryAdd(key, value);
            value = dictionary[key];
        }

        return value;
    }

    public static (TValue value, bool created) FindOrCreateWithStatus<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> createValue)
        where TValue : class
    {
        // Try to get the value from the dictionary
        if (!dictionary.TryGetValue(key, out TValue? value))
        {
            // Value not found, create a new value
            value = createValue(key);

            // Attempt to add the new value to the dictionary
            bool added = dictionary.TryAdd(key, value);

            // Return the value and whether it was added (created) or not
            return (value, added);
        }

        // Value found, return it and indicate it was not created
        return (value, false);
    }
}
