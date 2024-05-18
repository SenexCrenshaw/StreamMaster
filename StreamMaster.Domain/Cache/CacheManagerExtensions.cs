using Microsoft.Extensions.Caching.Memory;

namespace StreamMaster.Domain.Cache;

public static partial class CacheManagerExtensions
{

    public static readonly MemoryCacheEntryOptions NeverRemoveCacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);

    public static void RemoveChannelGroupStreamCount(this IMemoryCache cache, int channelGroupId)
    {
        lock (_lock)
        {
            List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
            ChannelGroupStreamCount? data = cachedData.Find(x => x.Id == channelGroupId);
            if (data != null)
            {
                _ = cachedData.Remove(data);
                _ = cache.Set(ChannelGroupStreamCountsConfig.Key, cachedData, NeverRemoveCacheEntryOptions);
            }
        }
    }

    public static void Remove(this IMemoryCache cache, object data)
    {
        if (data.GetType() == typeof(ChannelGroupStreamCount))
        {
            lock (_lock)
            {
                List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
                _ = cachedData.Remove((ChannelGroupStreamCount)data);
                _ = cache.Set(ChannelGroupStreamCountsConfig.Key, cachedData, NeverRemoveCacheEntryOptions);
            }
            return;
        }
    }



    //public static void AddOrUpdateChannelGroupSMStreamCounts2(this IMemoryCache cache, List<ChannelGroup> responses)
    //{
    //    foreach (ChannelGroup response in responses)
    //    {
    //        cache.AddOrUpdateChannelGroupSMStreamCounts(response);
    //    }
    //}

    //public static void UpdateChannelGroupWithActives(this IMemoryCache cache, ChannelGroup channelGroup)
    //{
    //    List<ChannelGroupStreamCount> test = cache.ChannelGroupStreamCounts();

    //    ChannelGroupStreamCount? active = cache.ChannelGroupStreamCounts().FirstOrDefault(a => a.Id == channelGroup.Id);
    //    if (active == null)
    //    {
    //        return;
    //    }

    //    channelGroup.ActiveCount = active.ActiveCount;
    //    channelGroup.HiddenCount = active.HiddenCount;
    //    channelGroup.TotalCount = active.TotalCount;
    //}

    //public static List<ChannelGroup> UpdateChannelGroupsWithActives(this IMemoryCache cache, List<ChannelGroup> channelGroups)
    //{
    //    for (int i = 0; i < channelGroups.Count; i++)
    //    {
    //        cache.UpdateChannelGroupWithActives(channelGroups[i]);
    //    }
    //    return channelGroups;
    //}

    //public static void AddChannelGroupStreamCount2(this IMemoryCache cache, ChannelGroupStreamCount ChannelGroup)
    //{
    //    List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
    //    cachedData.Add(ChannelGroup);

    //    _ = cache.Set(ChannelGroupStreamCountsConfig.Key, cachedData, NeverRemoveCacheEntryOptions);
    //}

    //public static void AddOrUpdateChannelGroupSMStreamCounts2(this IMemoryCache cache, ChannelGroup ChannelGroup)
    //{
    //    List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
    //    ChannelGroupStreamCount? data = cachedData.Find(a => a.Id == ChannelGroup.Id);

    //    if (data == null)
    //    {
    //        cache.AddChannelGroupStreamCount2(ChannelGroup);
    //    }
    //    else
    //    {
    //        _ = cachedData.Remove(data);
    //        data.ActiveCount = ChannelGroup.ActiveCount;
    //        data.TotalCount = ChannelGroup.TotalCount;
    //        data.HiddenCount = ChannelGroup.HiddenCount;
    //        cachedData.Add(data);
    //        _ = cache.Set(ChannelGroupStreamCountsConfig.Key, cachedData, NeverRemoveCacheEntryOptions);
    //    }
    //}

    private static readonly object _lock = new();


    //public static Setting GetSetting(this IMemoryCache cache)
    //{
    //    _ = cache.TryGetValue(SettingConfig.Key, out Setting? settings);
    //    return settings ?? new Setting();
    //    }

    public static List<ChannelGroupStreamCount> ChannelGroupStreamCounts(this IMemoryCache cache)
    {
        if (cache.GetListFromCache<ChannelGroupStreamCount>(ChannelGroupStreamCountsConfig.Key) == null)
        {
            _ = cache.Set(ChannelGroupStreamCountsConfig.Key, new List<ChannelGroupStreamCount>());
        }
        return cache.GetListFromCache<ChannelGroupStreamCount>(ChannelGroupStreamCountsConfig.Key);
    }

    public static ChannelGroupStreamCount? GetChannelGroupVideoStreamCount(this IMemoryCache cache, int channelGroupId)
    {
        ChannelGroupStreamCount? ret = cache.ChannelGroupStreamCounts().Find(a => a.Id == channelGroupId);
        return ret;
    }
    //public static List<TvLogoFile> GetTvLogos(this IMemoryCache cache)
    //{
    //    return cache.GetListFromCache<TvLogoFile>(TVLogosConfig.Key);
    //}

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


    //public static void SetSetting(this IMemoryCache cache, Setting setting)
    //{
    //    lock (SettingConfig.Lock)
    //    {
    //        _ = cache.Set(SettingConfig.Key, setting, SettingConfig.CacheEntryOptions);
    //    }
    //}
}
