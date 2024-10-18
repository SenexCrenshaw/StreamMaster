namespace StreamMaster.Domain.Cache
{
    public interface ICachedConcurrentDictionary<TKey, TValue> where TKey : notnull
    {
        IEnumerable<TValue> Values { get; }

        void Clear();
        bool ContainsKey(TKey key);
        bool TryAdd(TKey key, TValue value);
        bool TryGetValue(TKey key, out TValue? value);
        bool TryRemove(TKey key, out TValue? value);
    }
}