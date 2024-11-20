namespace StreamMaster.Domain.Extensions;

public static class DictionaryExtensions
{
    public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue? value) where TKey : notnull
    {
        if (value is null)
        {
            return;
        }

        if (!dictionary.TryAdd(key, value))
        {
            dictionary[key] = value;
        }
    }
}