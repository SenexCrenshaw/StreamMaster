using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;
using StreamMaster.SchedulesDirectAPI.Domain.JsonClasses;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;

namespace StreamMasterDomain.Cache;

public static class CacheManagerExtensions  
{
    //private const string IsSystemReadyKey = "IsSystemReady";
    private const string ListChannelLogos = "ListChannelLogos";
    private const string ListIconFiles = "ListIconFiles";
    private const string ListProgrammeChannel = "ListProgrammeChannel";
    private const string ListProgrammes = "ListProgrammes";
    private const string ListSDProgrammes = "ListSDProgrammes";
    private const string ListProgrammesLogos = "ListProgrammesLogos";
    private const string ListTVLogos = "ListTVLogos";
    private const string ListChannelGroupStreamCounts = "ListChannelGroupStreamCounts";
    private const string SettingKey = "Setting";
    private const string ListImageInfos = "ListImageInfos";
    //private const string ListAffiliates = "ListAffiliates";

    public static readonly MemoryCacheEntryOptions NeverRemoveCacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);

  

    public static void Add(this IMemoryCache cache, object data)
    {
        if (data.GetType() == typeof(IconFileDto))
        {
            lock (_lock)
            {
                List<IconFileDto> cachedData = cache.GetListFromCache<IconFileDto>(ListIconFiles);
                cachedData.Add((IconFileDto)data);

                _ = cache.Set(ListIconFiles, cachedData, NeverRemoveCacheEntryOptions);
            }
            return;
        }

        if (data.GetType() == typeof(ImageInfo))
        {
            lock (_lock)
            {
                List<ImageInfo> cachedData = cache.GetListFromCache<ImageInfo>(ListImageInfos);
                cachedData.Add((ImageInfo)data);

                _ = cache.Set(ListImageInfos, cachedData, NeverRemoveCacheEntryOptions);
            }
            return;
        }

        if (data.GetType() == typeof(ChannelGroupStreamCount))
        {
            lock (_lock)
            {
                List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
                cachedData.Add((ChannelGroupStreamCount)data);

                _ = cache.Set(ListChannelGroupStreamCounts, cachedData, NeverRemoveCacheEntryOptions);
            }
            return;
        }

        if (data.GetType() == typeof(ChannelLogoDto))
        {
            List<ChannelLogoDto> cachedData = cache.GetListFromCache<ChannelLogoDto>(ListChannelLogos);
            cachedData.Add((ChannelLogoDto)data);
            _ = cache.Set(ListChannelLogos, cachedData, NeverRemoveCacheEntryOptions);
            return;
        }
    }

    public static void RemoveChannelGroupStreamCount(this IMemoryCache cache, int channelGroupId)
    {
        lock (_lock)
        {
            List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
            ChannelGroupStreamCount? data = cachedData.Find(x => x.ChannelGroupId == channelGroupId);
            if (data != null)
            {
                _ = cachedData.Remove(data);
                _ = cache.Set(ListChannelGroupStreamCounts, cachedData, NeverRemoveCacheEntryOptions);
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
                _ = cache.Set(ListChannelGroupStreamCounts, cachedData, NeverRemoveCacheEntryOptions);
            }
            return;
        }
    }

    public static void AddProgrammeLogo(this IMemoryCache cache, IconFileDto icon)
    {
        List<IconFileDto> cachedData = cache.GetListFromCache<IconFileDto>(ListProgrammesLogos);
        cachedData.Add(icon);
        _ = cache.Set(ListProgrammesLogos, cachedData, NeverRemoveCacheEntryOptions);
        return;
    }


    public static void ClearChannelLogos(this IMemoryCache cache)
    {
        cache.Remove(ListChannelLogos);
    }

    public static void ClearIcons(this IMemoryCache cache)
    {
        cache.Remove(ListIconFiles);
    }

    public static void ClearProgrammeChannels(this IMemoryCache cache)
    {
        cache.Remove(ListProgrammeChannel);
    }

    public static void ClearSDProgrammes(this IMemoryCache cache)
    {
        cache.Remove(ListSDProgrammes);
    }

    public static void ClearProgrammes(this IMemoryCache cache)
    {
        cache.Remove(ListProgrammes);
    }

    public static void ClearTvLogos(this IMemoryCache cache)
    {
        cache.Remove(ListTVLogos);
    }

    public static List<IconFileDto> GetIcons(this IMemoryCache cache, IMapper mapper)
    {
        //if (!cache.TryGetValue(ListIconFiles, out List<IconFileDto>? cacheValue))
        //{
        List<IconFileDto> cacheValue = mapper.Map<List<IconFileDto>>(cache.TvLogos());
        //List<ChannelLogoDto> a = cache.ChannelLogos();
        cacheValue.AddRange(cache.Icons());
        //cacheValue.AddRange(mapper.Map<List<IconFileDto>>(cache.ChannelLogos()));

        //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
        //    .SetPriority(CacheItemPriority.NeverRemove);

        //_ = cache.Set(ListIconFiles, cacheValue, cacheEntryOptions);
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

    public static void AddOrUpdateChannelGroupVideoStreamCount(this IMemoryCache cache, ChannelGroupDto ChannelGroupDto)
    {
        List<ChannelGroupStreamCount> cachedData = cache.ChannelGroupStreamCounts();
        ChannelGroupStreamCount? data = cachedData.Find(a => a.ChannelGroupId == ChannelGroupDto.Id);

        if (data == null)
        {
            cache.Add(ChannelGroupDto);
        }
        else
        {
            _ = cachedData.Remove(data);
            data.ActiveCount = ChannelGroupDto.ActiveCount;
            data.TotalCount = ChannelGroupDto.TotalCount;
            data.HiddenCount = ChannelGroupDto.HiddenCount;
            cachedData.Add(data);
            _ = cache.Set(ListChannelGroupStreamCounts, cachedData, NeverRemoveCacheEntryOptions);
        }
    }

    private static readonly object _lock = new();


    #region Gets
    public static Setting GetSetting(this IMemoryCache cache)
    {
        _ = cache.TryGetValue(SettingKey, out Setting? settings);
        return settings ?? new Setting();
    }

    public static bool IsSystemReady(this IMemoryCache cache)
    {
        return cache.GetFromCache<bool?>(IsSystemReadyConfig.Key) ?? false;       
    }

    //public static List<MxfAffiliate> GetAffiliates(this IMemoryCache cache)
    //{
    //    return cache.GetListFromCache<MxfAffiliate>(AffiliateConfig.Key);
    //}

    //public static MxfAffiliate? GetAffiliate(this IMemoryCache cache, string affiliateName)
    //{
    //    return cache.GetListFromCache<MxfAffiliate>(AffiliateConfig.Key).Find(a => a.Name == affiliateName);
    //}

    //public static List<MxfService> GetServices(this IMemoryCache cache)
    //{
    //    return cache.GetListFromCache<MxfService>(StationConfig.Key);
    //}

    //public static MxfService? GetService(this IMemoryCache cache, string stationId)
    //{
    //    return cache.GetListFromCache<MxfService>(StationConfig.Key).Find(a => a.StationId == stationId);
    //}

    //public static List<MxfGuideImage> GetGuideImages(this IMemoryCache cache)
    //{
    //    return cache.GetListFromCache<MxfGuideImage>(GuideImageConfig.Key);
    //}

    //public static MxfGuideImage? GetGuideImage(this IMemoryCache cache, string pathname)
    //{
    //    return cache.GetListFromCache<MxfGuideImage>(GuideImageConfig.Key).Find(a => a.ImageUrl == pathname);
    //}

    //public static List<MxfLineup> GetLineUps(this IMemoryCache cache)
    //{
    //    return cache.GetListFromCache<MxfLineup>(LineupConfig.Key);
    //}

    //public static MxfLineup? GetLineUp(this IMemoryCache cache, string lineupId)
    //{
    //    return cache.GetListFromCache<MxfLineup>(LineupConfig.Key).Find(a => a.LineupId == lineupId);
    //}

    //public static List<KeyValuePair<MxfService, string[]>> GetStationLogos(this IMemoryCache cache)
    //{
    //    return cache.GetListFromCache<KeyValuePair<MxfService, string[]>>(StationLogoConfig.Key);
    //}

    //public static KeyValuePair<MxfService, string[]?> GetStationLogo(this IMemoryCache cache, string url)
    //{
    //    return cache.GetListFromCache<KeyValuePair<MxfService, string[]?>>(StationLogoConfig.Key).Find(a => a == url);
    //}

    public static List<ImageInfo> ImageInfos(this IMemoryCache cache)
    {
        return cache.GetListFromCache<ImageInfo>(ListImageInfos);
    }

    public static List<ChannelLogoDto> ChannelLogos(this IMemoryCache cache)
    {
        return cache.GetListFromCache<ChannelLogoDto>(ListChannelLogos);
    }
    public static List<IconFileDto> Icons(this IMemoryCache cache)
    {
        return cache.GetListFromCache<IconFileDto>(ListIconFiles);
    }

    public static List<ChannelGroupStreamCount> ChannelGroupStreamCounts(this IMemoryCache cache)
    {
        return cache.GetListFromCache<ChannelGroupStreamCount>(ListChannelGroupStreamCounts);
    }

    public static IconFileDto? GetIcon(this IMemoryCache cache, string source, SMFileTypes sMFileTypes)
    {
        lock (_lock)
        {
            IconFileDto? testIcon = cache.Icons().Find(a => a.Source == source && a.SMFileType == sMFileTypes);
            return testIcon;
        }
    }

    public static List<ProgrammeChannel> ProgrammeChannels(this IMemoryCache cache)
    {
        return cache.GetListFromCache<ProgrammeChannel>(ListProgrammeChannel);
    }

    public static List<IconFileDto> ProgrammeIcons(this IMemoryCache cache)
    {
        return cache.GetListFromCache<IconFileDto>(ListProgrammesLogos);
    }

    public static ChannelGroupStreamCount? GetChannelGroupVideoStreamCount(this IMemoryCache cache, int channelGroupId)
    {
        ChannelGroupStreamCount? ret = cache.ChannelGroupStreamCounts().Find(a => a.ChannelGroupId == channelGroupId);
        return ret;
    }

    public static List<EPGProgramme> Programmes(this IMemoryCache cache)
    {
        return cache.GetListFromCache<EPGProgramme>(ListProgrammes);
    }

    public static List<EPGProgramme> SDProgrammess(this IMemoryCache cache)
    {
        return cache.GetListFromCache<EPGProgramme>(ListSDProgrammes );
    }

    public static List<TvLogoFile> TvLogos(this IMemoryCache cache)
    {
        return cache.GetListFromCache<TvLogoFile>(ListTVLogos );
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

        var ret = new List<T>();
        cache.Set(key, ret, NeverRemoveCacheEntryOptions);
        return ret;
    }

    #endregion

    #region Sets

    //public static bool AreProgrammeListsEqual(List<Programme> list1, List<Programme> list2)
    //{
    //    if (list1 == null || list2 == null)
    //    {
    //        throw new ArgumentNullException(nameof(list1));
    //    }

    //    if (list1.Count != list2.Count)
    //    {
    //        return false;
    //    }

    //    ProgrammeNameStartComparer comparer = new();
    //    HashSet<Programme> set = new(list1, comparer);

    //    return list2.All(set.Contains);
    //}

    //public static bool SetSDProgreammesCache(this IMemoryCache cache, List<Programme> data, TimeSpan? expiration = null)
    //{
    //    MemoryCacheEntryOptions CacheEntryOptions = expiration == null
    //        ? new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove)
    //        : new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };

    //    if (!AreProgrammeListsEqual(cache.SDProgrammess(), data))
    //    {
    //        _ = cache.Set(ListSDProgrammes, data, CacheEntryOptions);
    //        return true;
    //    }
    //    return false;
    //}

    
    //     public static void AddLineUp(this IMemoryCache cache, MxfLineup lineUp)
    //{
    //    lock (LineupConfig.Lock)
    //    {
    //        List<MxfLineup> lineUps = cache.GetLineUps();
    //        if (lineUps.Any(a => a.LineupId == lineUp.LineupId))
    //        {
    //            return;
    //        }

    //        lineUps.Add(lineUp);
    //        _ = cache.Set(LineupConfig.Key, lineUps, NeverRemoveCacheEntryOptions);
    //    }
    //}

    //public static void AddAffiliate(this IMemoryCache cache, MxfAffiliate affiliate)
    //{
    //    lock (AffiliateConfig.Lock)
    //    {
    //        List<MxfAffiliate> affiliates = cache.GetAffiliates();
    //        if (affiliates.Any(a => a.Name == affiliate.Name))
    //        {
    //            return;
    //        }

    //        affiliates.Add(affiliate);
    //        _ = cache.Set(AffiliateConfig.Key, affiliates, NeverRemoveCacheEntryOptions);
    //    }
    //}

    //public static void AddGuideImage(this IMemoryCache cache, MxfGuideImage guideImage)
    //{
    //    lock (GuideImageConfig.Lock)
    //    {
    //        var guideImages = cache.GetGuideImages();
    //        if (guideImages.Any(a => a.ImageUrl == guideImage.ImageUrl))
    //        {
    //            return;
    //        }

    //        guideImages.Add(guideImage);
    //        _ = cache.Set(GuideImageConfig.Key, guideImages, NeverRemoveCacheEntryOptions);
    //    }
    //}

    //public static void AddService(this IMemoryCache cache, MxfService service)
    //{
    //    lock (StationConfig.Lock)
    //    {
    //        var services = cache.GetServices();
    //        if (services.Any(a => a.StationId == service.StationId))
    //        {
    //            return;
    //        }

    //        services.Add(service);
    //        _ = cache.Set(StationConfig.Key, services, NeverRemoveCacheEntryOptions);
    //    }
    //}

    public static void SetCache(this IMemoryCache cache, object data, TimeSpan? expiration = null)
    {
        MemoryCacheEntryOptions CacheEntryOptions = expiration == null
            ? new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove)
            : new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
        if (data.GetType().GenericTypeArguments.Contains(typeof(IconFileDto)))
        {
            _ = cache.Set(ListIconFiles, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(TvLogoFile)))
        {
            _ = cache.Set(ListTVLogos, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(ImageInfo)))
        {
            _ = cache.Set(ListImageInfos, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(ChannelLogoDto)))
        {
            _ = cache.Set(ListChannelLogos, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(Programme)))
        {
            _ = cache.Set(ListProgrammes, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(ProgrammeChannel)))
        {
            _ = cache.Set(ListProgrammeChannel, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(Programme)))
        {
            _ = cache.Set(ListProgrammeChannel, data, CacheEntryOptions);
            return;
        }

        _ = data.GetType().Name;
        throw new Exception($"Cache set Unknown type {data.GetType().Name}");
    }

    public static void SetIsSystemReady(this IMemoryCache cache, bool isSystemReady)
    {
        _ = cache.Set(IsSystemReadyConfig.Key, isSystemReady, IsSystemReadyConfig.CacheEntryOptions);
    }

    public static void SetProgrammeLogos(this IMemoryCache cache, List<IconFileDto> icons)
    {
        _ = cache.Set(ListProgrammesLogos, icons, NeverRemoveCacheEntryOptions);
        return;
    }
    #endregion

}