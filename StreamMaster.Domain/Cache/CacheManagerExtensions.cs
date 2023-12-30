using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Cache;

public static partial class CacheManagerExtensions
{

    public static readonly MemoryCacheEntryOptions NeverRemoveCacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);

    public static void RemoveChannelGroupStreamCount(this IMemoryCache cache, int channelGroupId)
    {
        lock (_lock)
        {
            List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
            ChannelGroupStreamCount? data = cachedData.Find(x => x.ChannelGroupId == channelGroupId);
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



    public static void AddOrUpdateChannelGroupVideoStreamCounts(this IMemoryCache cache, List<ChannelGroupDto> responses)
    {
        foreach (ChannelGroupDto response in responses)
        {
            cache.AddOrUpdateChannelGroupVideoStreamCount(response);
        }
    }

    public static void UpdateChannelGroupWithActives(this IMemoryCache cache, ChannelGroupDto channelGroup)
    {
        ChannelGroupStreamCount? active = cache.ChannelGroupStreamCounts().Find(a => a.ChannelGroupId == channelGroup.Id);
        if (active == null)
        {
            return;
        }

        channelGroup.ActiveCount = active.ActiveCount;
        channelGroup.HiddenCount = active.HiddenCount;
        channelGroup.TotalCount = active.TotalCount;
    }

    public static List<ChannelGroupDto> UpdateChannelGroupsWithActives(this IMemoryCache cache, List<ChannelGroupDto> channelGroups)
    {
        for (int i = 0; i < channelGroups.Count; i++)
        {
            cache.UpdateChannelGroupWithActives(channelGroups[i]);
        }
        return channelGroups;
    }

    public static void AddChannelGroupStreamCount(this IMemoryCache cache, ChannelGroupStreamCount ChannelGroupDto)
    {
        List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
        cachedData.Add(ChannelGroupDto);

        _ = cache.Set(ChannelGroupStreamCountsConfig.Key, cachedData, NeverRemoveCacheEntryOptions);
    }

    public static void AddOrUpdateChannelGroupVideoStreamCount(this IMemoryCache cache, ChannelGroupDto ChannelGroupDto)
    {
        List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
        ChannelGroupStreamCount? data = cachedData.Find(a => a.ChannelGroupId == ChannelGroupDto.Id);

        if (data == null)
        {
            cache.AddChannelGroupStreamCount(ChannelGroupDto);
        }
        else
        {
            _ = cachedData.Remove(data);
            data.ActiveCount = ChannelGroupDto.ActiveCount;
            data.TotalCount = ChannelGroupDto.TotalCount;
            data.HiddenCount = ChannelGroupDto.HiddenCount;
            cachedData.Add(data);
            _ = cache.Set(ChannelGroupStreamCountsConfig.Key, cachedData, NeverRemoveCacheEntryOptions);
        }
    }

    private static readonly object _lock = new();


    public static Setting GetSetting(this IMemoryCache cache)
    {
        _ = cache.TryGetValue(SettingConfig.Key, out Setting? settings);
        return settings ?? new Setting();
    }

    public static List<ChannelGroupStreamCount> ChannelGroupStreamCounts(this IMemoryCache cache)
    {
        return cache.GetListFromCache<ChannelGroupStreamCount>(ChannelGroupStreamCountsConfig.Key);
    }

    public static ChannelGroupStreamCount? GetChannelGroupVideoStreamCount(this IMemoryCache cache, int channelGroupId)
    {
        ChannelGroupStreamCount? ret = cache.ChannelGroupStreamCounts().Find(a => a.ChannelGroupId == channelGroupId);
        return ret;
    }
    public static List<TvLogoFile> GetTvLogos(this IMemoryCache cache)
    {
        return cache.GetListFromCache<TvLogoFile>(TVLogosConfig.Key);
    }

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


    public static void SetSetting(this IMemoryCache cache, Setting setting)
    {
        lock (SettingConfig.Lock)
        {
            _ = cache.Set(SettingConfig.Key, setting, SettingConfig.CacheEntryOptions);
        }
    }
}
