using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Cache;

public static class CacheManagerExtensions
{

    public static readonly MemoryCacheEntryOptions NeverRemoveCacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);

    //public static void Add(this IMemoryCache cache, object data)
    //{
    //    if (data.GetType() == typeof(IconFileDto))
    //    {
    //        lock (_lock)
    //        {
    //            List<IconFileDto> cachedData = cache.GetListFromCache<IconFileDto>(IconsConfig.Key);
    //            cachedData.Add((IconFileDto)data);

    //            _ = cache.Set(IconsConfig.Key, cachedData, NeverRemoveCacheEntryOptions);
    //        }
    //        return;
    //    }

    //    if (data.GetType() == typeof(ChannelGroupStreamCount))
    //    {
    //        lock (_lock)
    //        {
    //            List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
    //            cachedData.Add((ChannelGroupStreamCount)data);

    //            _ = cache.Set(ChannelGroupStreamCountsConfig.Key, cachedData, NeverRemoveCacheEntryOptions);
    //        }
    //        return;
    //    }

    //    if (data.GetType() == typeof(ChannelLogoDto))
    //    {
    //        List<ChannelLogoDto> cachedData = cache.GetListFromCache<ChannelLogoDto>(ListChannelLogos);
    //        cachedData.Add((ChannelLogoDto)data);
    //        _ = cache.Set(ListChannelLogos, cachedData, NeverRemoveCacheEntryOptions);
    //        return;
    //    }
    //}

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

    public static void ClearIcons(this IMemoryCache cache)
    {
        cache.Remove(IconsConfig.Key);
    }

    public static void ClearTvLogos(this IMemoryCache cache)
    {
        cache.Remove(TVLogosConfig.Key);
    }

    public static List<IconFileDto> GetIcons(this IMemoryCache cache, IMapper mapper)
    {
        //if (!cache.TryGetValue(IconsConfig.Key, out List<IconFileDto>? cacheValue))
        //{
        List<IconFileDto> cacheValue = mapper.Map<List<IconFileDto>>(cache.GetTvLogos());
        //List<ChannelLogoDto> a = cache.ChannelLogos();
        cacheValue.AddRange(cache.Icons());
        //cacheValue.AddRange(mapper.Map<List<IconFileDto>>(cache.ChannelLogos()));

        //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
        //    .SetPriority(CacheItemPriority.NeverRemove);

        //_ = cache.Set(IconsConfig.Key, cacheValue, cacheEntryOptions);
        //}
        int index = 0;
        IOrderedEnumerable<IconFileDto> ret = cacheValue.OrderBy(a => a.Name);
        foreach (IconFileDto? c in ret)
        {
            c.Id = index++;
        }

        return [.. ret];
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


    #region Gets
    public static Setting GetSetting(this IMemoryCache cache)
    {
        _ = cache.TryGetValue(SettingConfig.Key, out Setting? settings);
        return settings ?? new Setting();
    }

    public static bool IsSystemReady(this IMemoryCache cache)
    {
        return cache.GetFromCache<bool?>(IsSystemReadyConfig.Key) ?? false;
    }

    public static JobStatus GetSyncJobStatus(this IMemoryCache cache)
    {
        JobStatus? ret = cache.GetFromCache<JobStatus?>(EPGSyncConfig.Key);
        if (ret is null)
        {
            ret = new JobStatus();
            cache.SetSyncJobStatus(ret);
        }
        return ret;
    }

    public static void SetSyncSuccessful(this IMemoryCache cache)
    {
        lock (EPGSyncConfig.Lock)
        {
            JobStatus jobStatus = cache.GetSyncJobStatus();
            jobStatus.LastRun = DateTime.Now;
            jobStatus.IsRunning = false;
            jobStatus.LastSuccessful = jobStatus.LastRun;
            jobStatus.ForceNextRun = false;
            cache.Set(EPGSyncConfig.Key, jobStatus, EPGSyncConfig.CacheEntryOptions);
        }
    }
    public static void SetSyncError(this IMemoryCache cache)
    {
        lock (EPGSyncConfig.Lock)
        {
            JobStatus jobStatus = cache.GetSyncJobStatus();
            jobStatus.LastRun = DateTime.Now;
            jobStatus.LastError = jobStatus.LastRun;
            jobStatus.IsRunning = false;
            jobStatus.ForceNextRun = false;
            cache.Set(EPGSyncConfig.Key, jobStatus, EPGSyncConfig.CacheEntryOptions);
        }
    }


    public static void SetSyncForceNextRun(this IMemoryCache cache, bool Extra = false)
    {
        lock (EPGSyncConfig.Lock)
        {
            JobStatus jobStatus = cache.GetSyncJobStatus();
            jobStatus.ForceNextRun = true;
            if (Extra == true)
            {
                jobStatus.Extra = true;
            }
            cache.Set(EPGSyncConfig.Key, jobStatus, EPGSyncConfig.CacheEntryOptions);
        }
    }


    private static void SetSyncJobStatus(this IMemoryCache cache, JobStatus value)
    {
        lock (SettingConfig.Lock)
        {
            _ = cache.Set(EPGSyncConfig.Key, value, EPGSyncConfig.CacheEntryOptions);
        }
    }

    //public static SDTokenFile? GetSDToken(this IMemoryCache cache)
    //{
    //    return cache.GetFromCache<SDTokenFile>(SDTokenConfig.Key);
    //}

    //public static UserStatus? GetSDUserStatus(this IMemoryCache cache)
    //{
    //    return cache.GetFromCache<UserStatus>(SDUserStatusConfig.Key);
    //}

    public static List<IconFileDto> Icons(this IMemoryCache cache)
    {
        return cache.GetListFromCache<IconFileDto>(IconsConfig.Key);
    }

    public static List<ChannelGroupStreamCount> ChannelGroupStreamCounts(this IMemoryCache cache)
    {
        return cache.GetListFromCache<ChannelGroupStreamCount>(ChannelGroupStreamCountsConfig.Key);
    }

    public static IconFileDto? GetIcon(this IMemoryCache cache, string source, SMFileTypes sMFileTypes)
    {
        lock (_lock)
        {
            IconFileDto? testIcon = cache.Icons().Find(a => a.Source == source && a.SMFileType == sMFileTypes);
            return testIcon;
        }
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

    #endregion

    #region Sets

    //public static void SetSDToken(this IMemoryCache cache, SDTokenFile sdTokenFile)
    //{
    //    lock (SDTokenConfig.Lock)
    //    {
    //        _ = cache.Set(SDTokenConfig.Key, sdTokenFile, SDTokenConfig.CacheEntryOptions);
    //    }
    //}
    //public static void SetSDUserStatus(this IMemoryCache cache, UserStatus? userStatus)
    //{
    //    lock (SDUserStatusConfig.Lock)
    //    {
    //        _ = cache.Set(SDUserStatusConfig.Key, userStatus, SDUserStatusConfig.CacheEntryOptions);
    //    }
    //}

    public static void SetTvLogos(this IMemoryCache cache, List<TvLogoFile> icons)
    {
        lock (TVLogosConfig.Lock)
        {
            _ = cache.Set(TVLogosConfig.Key, icons, TVLogosConfig.CacheEntryOptions);
        }
    }

    public static void SetSetting(this IMemoryCache cache, Setting setting)
    {
        lock (SettingConfig.Lock)
        {
            _ = cache.Set(SettingConfig.Key, setting, SettingConfig.CacheEntryOptions);
        }
    }

    public static void SetIcons(this IMemoryCache cache, List<IconFileDto> icons)
    {
        lock (IconsConfig.Lock)
        {
            _ = cache.Set(IconsConfig.Key, icons, IconsConfig.CacheEntryOptions);
        }
    }

    //public static void SetCache(this IMemoryCache cache, object data, TimeSpan? expiration = null)
    //{
    //    MemoryCacheEntryOptions CacheEntryOptions = expiration == null
    //        ? new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove)
    //        : new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
    //    if (data.GetType().GenericTypeArguments.Contains(typeof(IconFileDto)))
    //    {
    //        _ = cache.Set(IconsConfig.Key, data, CacheEntryOptions);
    //        return;
    //    }

    //    if (data.GetType().GenericTypeArguments.Contains(typeof(TvLogoFile)))
    //    {
    //        _ = cache.Set(ListTVLogos, data, CacheEntryOptions);
    //        return;
    //    }


    //    if (data.GetType().GenericTypeArguments.Contains(typeof(ChannelLogoDto)))
    //    {
    //        _ = cache.Set(ListChannelLogos, data, CacheEntryOptions);
    //        return;
    //    }


    //    _ = data.GetType().Name;
    //    throw new Exception($"Cache set Unknown type {data.GetType().Name}");
    //}

    public static void SetIsSystemReady(this IMemoryCache cache, bool isSystemReady)
    {
        _ = cache.Set(IsSystemReadyConfig.Key, isSystemReady, IsSystemReadyConfig.CacheEntryOptions);
    }

    #endregion

}