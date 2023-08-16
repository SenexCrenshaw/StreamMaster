using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterDomain.Cache;

public static class CacheKeys
{
    const string IsSystemReadyKey = "IsSystemReady";

    const string ListChannelLogos = "ListChannelLogos";
    const string ListIconFiles = "ListIconFiles";
    const string ListProgrammeChannel = "ListProgrammeChannel";

    const string ListProgrammes = "ListProgrammes";
    const string ListProgrammesLogos = "ListProgrammesLogos";
    const string ListTVLogos = "ListTVLogos";

    const string ListChannelGroupVideoStreamCounts = "ListChannelGroupVideoStreamCounts";

    static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);

    public static void Add(this IMemoryCache cache, object data)
    {
        if (data.GetType() == typeof(IconFileDto))
        {
            lock (_lock)
            {
                List<IconFileDto> datas = Get<IconFileDto>(ListIconFiles, cache);
                datas.Add((IconFileDto)data);

                cache.Set(ListIconFiles, datas, CacheEntryOptions);
            }
            return;
        }

        if (data.GetType() == typeof(ChannelLogoDto))
        {
            List<ChannelLogoDto> datas = Get<ChannelLogoDto>(ListChannelLogos, cache);
            datas.Add((ChannelLogoDto)data);
            cache.Set(ListChannelLogos, datas, CacheEntryOptions);
            return;
        }

        //if (data.GetType().GenericTypeArguments.Contains(typeof(Programme)))
        //{
        //    cache.Set(ListProgrammes, data, CacheEntryOptions);
        //    return;
        //}

        //if (data.GetType().GenericTypeArguments.Contains(typeof(ProgrammeChannel)))
        //{
        //    cache.Set(ListProgrammeChannel, data, CacheEntryOptions);
        //    return;
        //}
    }
    public static bool ChannelGroupVideoStreamCountExists(this IMemoryCache cache, int id)
    {
        List<GetChannelGroupVideoStreamCountResponse> datas = Get<GetChannelGroupVideoStreamCountResponse>(ListChannelGroupVideoStreamCounts, cache);
        return datas.Any(a => a.Id == id);
    }
    public static void AddOrUpdateChannelGroupVideoStreamCount(this IMemoryCache cache, GetChannelGroupVideoStreamCountResponse response)
    {

        List<GetChannelGroupVideoStreamCountResponse> datas = Get<GetChannelGroupVideoStreamCountResponse>(ListChannelGroupVideoStreamCounts, cache);
        if (!datas.Any(a => a.Id == response.Id))
        {
            datas.Add(response);
        }
        else
        {
            datas.Remove(datas.First(a => a.Id == response.Id));
            datas.Add(response);
        }
        cache.Set(ListChannelGroupVideoStreamCounts, datas, CacheEntryOptions);
        return;
    }

    public static void AddProgrammeLogo(this IMemoryCache cache, IconFileDto icon)
    {
        List<IconFileDto> datas = Get<IconFileDto>(ListProgrammesLogos, cache);
        datas.Add(icon);
        cache.Set(ListProgrammesLogos, datas, CacheEntryOptions);
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

    public static void ClearProgrammes(this IMemoryCache cache)
    {
        cache.Remove(ListProgrammes);
    }

    public static void ClearTvLogos(this IMemoryCache cache)
    {
        cache.Remove(ListTVLogos);
    }

    public static string? GetEPGChannelByTvgId(this IMemoryCache cache, string User_Tvg_ID)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = cache.ProgrammeNames();

        List<ChannelLogoDto> channelLogos = cache.ChannelLogos();

        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == User_Tvg_ID);
        if (pn == null)
        {
            return null;
        }

        ChannelLogoDto? channelLogo = channelLogos.FirstOrDefault(a => a.EPGId == pn.Channel);
        if (channelLogo != null)
        {
            return channelLogo.LogoUrl;
        }
        return null;
    }

    public static string? GetEPGNameTvgName(this IMemoryCache cache, string User_Tvg_Name)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = cache.ProgrammeNames();

        List<ChannelLogoDto> channelLogos = cache.ChannelLogos();

        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == User_Tvg_Name);
        if (pn == null)
        {
            pn = programmeNames.FirstOrDefault(a => a.ChannelName == User_Tvg_Name);
            if (pn == null)
            {
                return null;
            }
        }
        return User_Tvg_Name;
    }

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

    public static GetChannelGroupVideoStreamCountResponse? GetChannelGroupVideoStreamCount(this IMemoryCache cache, int id)
    {
        return cache.GetChannelGroupVideoStreamCounts().FirstOrDefault(a => a.Id == id);
    }

    public static bool RemoveChannelGroupVideoStreamCount(this IMemoryCache cache, int id)
    {
        List<GetChannelGroupVideoStreamCountResponse> datas = cache.GetChannelGroupVideoStreamCounts();
        GetChannelGroupVideoStreamCountResponse? d = datas.FirstOrDefault(a => a.Id == id);

        if (d == null)
        {
            return true;
        }
        datas.Remove(d);
        cache.Set(ListChannelGroupVideoStreamCounts, datas, CacheEntryOptions);
        return true;
    }


    public static List<GetChannelGroupVideoStreamCountResponse> GetChannelGroupVideoStreamCounts(this IMemoryCache cache)
    {
        return Get<GetChannelGroupVideoStreamCountResponse>(ListChannelGroupVideoStreamCounts, cache);
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

    public static IEnumerable<ProgrammeNameDto> ProgrammeNames(this IMemoryCache cache)
    {
        List<Programme> programmes = cache.Programmes().Where(a => !string.IsNullOrEmpty(a.Channel) && a.StopDateTime > DateTime.Now.AddDays(-1)).ToList();
        if (programmes.Any())
        {
            IEnumerable<ProgrammeNameDto> ret = programmes.GroupBy(a => a.Channel).Select(group => group.First()).Select(a => new ProgrammeNameDto
            {
                Channel = a.Channel,
                ChannelName = a.ChannelName,
                DisplayName = a.DisplayName
            });

            return ret.OrderBy(a => a.DisplayName);
        }
        return new List<ProgrammeNameDto>();
    }

    public static List<Programme> Programmes(this IMemoryCache cache)
    {
        return Get<Programme>(ListProgrammes, cache);
    }

    public static void Set(this IMemoryCache cache, object data)
    {
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
        throw new Exception($"Cache set Unknown type {data.GetType().Name}");
    }

    public static void SetIsSystemReady(this IMemoryCache cache, bool isSystemReady)
    {
        cache.Set(IsSystemReadyKey, isSystemReady, CacheEntryOptions);
    }

    public static void SetProgrammeLogos(this IMemoryCache cache, List<IconFileDto> icons)
    {
        cache.Set(ListProgrammesLogos, icons, CacheEntryOptions);
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
