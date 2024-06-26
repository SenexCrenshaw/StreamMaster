using Microsoft.Extensions.Caching.Memory;

namespace StreamMaster.Domain.Cache;

public static partial class CacheManagerExtensions
{

    public static readonly MemoryCacheEntryOptions NeverRemoveCacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);



    private static readonly object _lock = new();

    private static T? GetFromCache<T>(this IMemoryCache cache, string key)
    {
        if (cache.TryGetValue(key, out T? cacheValue))
        {
            if (cacheValue != null)
            {
                return cacheValue;
            }
        }
        return default;
    }

    private static List<T> GetListFromCache<T>(this IMemoryCache cache, string key)
    {
        if (cache.TryGetValue(key, out List<T>? cacheValue))
        {
            if (cacheValue != null)
            {
                return cacheValue;
            }
        }

        List<T> ret = [];
        _ = cache.Set(key, ret, NeverRemoveCacheEntryOptions);
        return ret;
    }

}
