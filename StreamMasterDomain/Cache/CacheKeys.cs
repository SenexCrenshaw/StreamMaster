using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;
using StreamMasterDomain.EPG;
using StreamMasterDomain.Models;

namespace StreamMasterDomain.Cache;

public static class CacheKeys
{
    private const string IsSystemReadyKey = "IsSystemReady";
    private const string ListChannelLogos = "ListChannelLogos";
    private const string ListIconFiles = "ListIconFiles";
    private const string ListProgrammeChannel = "ListProgrammeChannel";
    private const string ListProgrammes = "ListProgrammes";
    private const string ListSDProgrammes = "ListSDProgrammes";
    private const string ListProgrammesLogos = "ListProgrammesLogos";
    private const string ListTVLogos = "ListTVLogos";
    private const string ListChannelGroupStreamCounts = "ListChannelGroupStreamCounts";
    private const string SettingKey = "Setting";

    private static readonly MemoryCacheEntryOptions DefaultCacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);

    public static Setting GetSetting(this IMemoryCache cache)
    {
        cache.TryGetValue(SettingKey, out Setting? settings);
        return settings ?? new Setting();
    }

    public static void Add(this IMemoryCache cache, object data)
    {
        if (data.GetType() == typeof(IconFileDto))
        {
            lock (_lock)
            {
                List<IconFileDto> datas = Get<IconFileDto>(ListIconFiles, cache);
                datas.Add((IconFileDto)data);

                cache.Set(ListIconFiles, datas, DefaultCacheEntryOptions);
            }
            return;
        }

        if (data.GetType() == typeof(ChannelGroupStreamCount))
        {
            lock (_lock)
            {
                List<ChannelGroupStreamCount> datas = cache.ChannelGroupStreamCounts();
                datas.Add((ChannelGroupStreamCount)data);

                cache.Set(ListChannelGroupStreamCounts, datas, DefaultCacheEntryOptions);
            }
            return;
        }

        if (data.GetType() == typeof(ChannelLogoDto))
        {
            List<ChannelLogoDto> datas = Get<ChannelLogoDto>(ListChannelLogos, cache);
            datas.Add((ChannelLogoDto)data);
            cache.Set(ListChannelLogos, datas, DefaultCacheEntryOptions);
            return;
        }
    }

    public static void RemoveChannelGroupStreamCount(this IMemoryCache cache, int channelGroupId)
    {
        lock (_lock)
        {
            List<ChannelGroupStreamCount> datas = cache.ChannelGroupStreamCounts();
            ChannelGroupStreamCount? data = datas.FirstOrDefault(x => x.ChannelGroupId == channelGroupId);
            if (data != null)
            {
                datas.Remove(data);
                cache.Set(ListChannelGroupStreamCounts, datas, DefaultCacheEntryOptions);
            }
        }
    }

    public static void Remove(this IMemoryCache cache, object data)
    {
        if (data.GetType() == typeof(ChannelGroupStreamCount))
        {
            lock (_lock)
            {
                List<ChannelGroupStreamCount> datas = cache.ChannelGroupStreamCounts();
                datas.Remove((ChannelGroupStreamCount)data);
                cache.Set(ListChannelGroupStreamCounts, datas, DefaultCacheEntryOptions);
            }
            return;
        }
    }

    public static void AddProgrammeLogo(this IMemoryCache cache, IconFileDto icon)
    {
        List<IconFileDto> datas = Get<IconFileDto>(ListProgrammesLogos, cache);
        datas.Add(icon);
        cache.Set(ListProgrammesLogos, datas, DefaultCacheEntryOptions);
        return;
    }

    public static List<ChannelLogoDto> ChannelLogos(this IMemoryCache cache)
    {
        return Get<ChannelLogoDto>(ListChannelLogos, cache);
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

    //public static string? GetEPGNameTvgName(this IMemoryCache cache, string User_Tvg_Name)
    //{
    //    IEnumerable<ProgrammeNameDto> programmeNames = cache.ProgrammeNames();

    //    ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == User_Tvg_Name);
    //    if (pn == null)
    //    {
    //        pn = programmeNames.FirstOrDefault(a => a.ChannelName == User_Tvg_Name);
    //        if (pn == null)
    //        {
    //            return null;
    //        }
    //    }
    //    return User_Tvg_Name;
    //}

    //public static string? GetEPGChannelLogoByTvgId(this IMemoryCache cache, string User_Tvg_ID)
    //{
    //    IEnumerable<ProgrammeNameDto> programmeNames = cache.ProgrammeNames();

    //    List<ChannelLogoDto> channelLogos = cache.ChannelLogos();

    //    ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == User_Tvg_ID);
    //    if (pn == null)
    //    {
    //        pn = programmeNames.FirstOrDefault(a => a.Channel == User_Tvg_ID);
    //        if (pn == null)
    //        {
    //            return null;
    //        }
    //    }

    //    ChannelLogoDto? channelLogo = channelLogos.FirstOrDefault(a => a.EPGId == pn.Channel);
    //    if (channelLogo != null)
    //    {
    //        return channelLogo.LogoUrl;
    //    }
    //    return null;
    //}

    //public static ProgrammeNameDto? GetEPGChannelByDisplayName(this IMemoryCache cache, string displayName)
    //{

    //    IEnumerable<ProgrammeNameDto> programmeNames = cache.ProgrammeNames();

    //    ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == displayName);
    //    if (pn == null)
    //    {
    //        pn = programmeNames.FirstOrDefault(a => a.ChannelName == displayName);
    //        if (pn == null)
    //        {
    //            return programmeNames.FirstOrDefault(a => a.Channel == displayName); ;
    //        }
    //    }
    //    return pn;
    //}

    //public static string? GetEPGChannelNameByDisplayName(this IMemoryCache cache, string displayName)
    //{
    //    IEnumerable<ProgrammeNameDto> programmeNames = cache.ProgrammeNames();

    //    ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == displayName);
    //    if (pn == null)
    //    {
    //        pn = programmeNames.FirstOrDefault(a => a.ChannelName == displayName);
    //        if (pn == null)
    //        {
    //            return null;
    //        }
    //    }
    //    return pn.Channel;
    //}

    public static List<IconFileDto> GetIcons(this IMemoryCache cache, IMapper mapper)
    {
        if (!cache.TryGetValue(ListIconFiles, out List<IconFileDto>? cacheValue))
        {
            cacheValue = mapper.Map<List<IconFileDto>>(cache.TvLogos());

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove);

            cache.Set(ListIconFiles, cacheValue, cacheEntryOptions);
        }

        return cacheValue ?? new List<IconFileDto>();
    }

    public static List<IconFileDto> Icons(this IMemoryCache cache)
    {
        return Get<IconFileDto>(ListIconFiles, cache);
    }

    public static List<ChannelGroupStreamCount> ChannelGroupStreamCounts(this IMemoryCache cache)
    {
        return Get<ChannelGroupStreamCount>(ListChannelGroupStreamCounts, cache);
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
        ChannelGroupStreamCount? active = cache.ChannelGroupStreamCounts().FirstOrDefault(a => a.ChannelGroupId == channelGroup.Id);
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
        List<ChannelGroupStreamCount> datas = cache.ChannelGroupStreamCounts();
        ChannelGroupStreamCount? data = datas.FirstOrDefault(a => a.ChannelGroupId == ChannelGroupDto.Id);

        if (data == null)
        {
            cache.Add(ChannelGroupDto);
        }
        else
        {
            datas.Remove(data);
            data.ActiveCount = ChannelGroupDto.ActiveCount;
            data.TotalCount = ChannelGroupDto.TotalCount;
            data.HiddenCount = ChannelGroupDto.HiddenCount;
            datas.Add(data);
            cache.Set(ListChannelGroupStreamCounts, datas, DefaultCacheEntryOptions);
        }
    }

    private static readonly object _lock = new();

    public static IconFileDto? GetIcon(this IMemoryCache cache, string source, SMFileTypes sMFileTypes)
    {
        lock (_lock)
        {
            IconFileDto? testIcon = cache.Icons().FirstOrDefault(a => a.Source == source && a.SMFileType == sMFileTypes);
            return testIcon;
        }
    }

    public static bool IsSystemReady(this IMemoryCache cache)
    {
        if (cache.TryGetValue(IsSystemReadyKey, out bool? cacheValue))
        {
            if (cacheValue != null)
            {
                return (bool)cacheValue;
            }
        }
        return false;
    }

    public static List<ProgrammeChannel> ProgrammeChannels(this IMemoryCache cache)
    {
        return Get<ProgrammeChannel>(ListProgrammeChannel, cache);
    }

    public static List<IconFileDto> ProgrammeIcons(this IMemoryCache cache)
    {
        return Get<IconFileDto>(ListProgrammesLogos, cache);
    }

    //public static IEnumerable<ProgrammeNameDto> ProgrammeNames(this IMemoryCache cache)
    //{
    //    List<Programme> programmes = cache.Programmess().Where(a => !string.IsNullOrEmpty(a.Channel)).ToList();
    //    if (programmes.Any())
    //    {
    //        IEnumerable<ProgrammeNameDto> ret = programmes.GroupBy(a => a.Channel).Select(group => group.First()).Select(a => new ProgrammeNameDto
    //        {
    //            Channel = a.Channel,
    //            ChannelName = a.ChannelName,
    //            DisplayName = a.DisplayName
    //        });

    //        return ret.OrderBy(a => a.DisplayName);
    //    }
    //    return new List<ProgrammeNameDto>();
    //}

    public static ChannelGroupStreamCount? GetChannelGroupVideoStreamCount(this IMemoryCache cache, int channelGroupId)
    {
        ChannelGroupStreamCount? ret = cache.ChannelGroupStreamCounts().FirstOrDefault(a => a.ChannelGroupId == channelGroupId);
        return ret;
    }

    public static List<Programme> Programmess(this IMemoryCache cache)
    {
        return Get<Programme>(ListProgrammes, cache);
    }

    public static List<Programme> SDProgrammess(this IMemoryCache cache)
    {
        return Get<Programme>(ListSDProgrammes, cache);
    }
    public static void SetSDProgreammesCache(this IMemoryCache cache, object data, TimeSpan? expiration = null)
    {
        MemoryCacheEntryOptions CacheEntryOptions;

        if (expiration == null)
        {
            CacheEntryOptions = new MemoryCacheEntryOptions { }.SetPriority(CacheItemPriority.NeverRemove);
        }
        else
        {
            CacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
        }
        cache.Set(ListSDProgrammes, data, CacheEntryOptions);

    }
    public static void SetCache(this IMemoryCache cache, object data, TimeSpan? expiration = null)
    {

        MemoryCacheEntryOptions CacheEntryOptions;

        if (expiration == null)
        {
            CacheEntryOptions = new MemoryCacheEntryOptions { }.SetPriority(CacheItemPriority.NeverRemove);
        }
        else
        {
            CacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
        }



        if (data.GetType().GenericTypeArguments.Contains(typeof(IconFileDto)))
        {
            cache.Set(ListIconFiles, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(TvLogoFile)))
        {
            cache.Set(ListTVLogos, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(ChannelLogoDto)))
        {
            cache.Set(ListChannelLogos, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(Programme)))
        {
            cache.Set(ListProgrammes, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(ProgrammeChannel)))
        {
            cache.Set(ListProgrammeChannel, data, CacheEntryOptions);
            return;
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(Programme)))
        {
            cache.Set(ListProgrammeChannel, data, CacheEntryOptions);
            return;
        }
        string aaa = data.GetType().Name;
        throw new Exception($"Cache set Unknown type {data.GetType().Name}");
    }

    public static void SetIsSystemReady(this IMemoryCache cache, bool isSystemReady)
    {
        cache.Set(IsSystemReadyKey, isSystemReady, DefaultCacheEntryOptions);
    }

    public static void SetProgrammeLogos(this IMemoryCache cache, List<IconFileDto> icons)
    {
        cache.Set(ListProgrammesLogos, icons, DefaultCacheEntryOptions);
        return;
    }

    public static List<TvLogoFile> TvLogos(this IMemoryCache cache)
    {
        return Get<TvLogoFile>(ListTVLogos, cache);
    }

    private static List<T> Get<T>(string key, IMemoryCache cache)
    {
        lock (_lock)
        {
            if (cache.TryGetValue(key, out List<T>? cacheValue))
            {
                if (cacheValue != null)
                {
                    return cacheValue;
                }
            }
            return new List<T>();
        }
    }
}