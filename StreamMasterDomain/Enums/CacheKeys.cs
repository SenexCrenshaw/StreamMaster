﻿using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities.EPG;

namespace StreamMasterDomain.Enums;

public static class CacheKeys
{
    public const string IsSystemReadyKey = "IsSystemReady";

    public const string ListChannelLogos = "ListChannelLogos";
    public const string ListIconFiles = "ListIconFileDto";
    public const string ListProgrammeChannel = "ListProgrammeChannel";

    public const string ListProgrammes = "ListProgrammes";
    public const string ListTVLogos = "ListTVLogos";
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);

    public static void Add(this IMemoryCache cache, object data)
    {
        if (data.GetType() == (typeof(IconFileDto)))
        {
            var datas = Get<IconFileDto>(ListIconFiles, cache);
            datas.Add((IconFileDto)data);
            cache.Set(ListIconFiles, datas, CacheEntryOptions);
            return;
        }

        if (data.GetType() == (typeof(ChannelLogoDto)))
        {
            var datas = Get<ChannelLogoDto>(ListChannelLogos, cache);
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
        var programmeNames = cache.ProgrammeNames();

        var channelLogos = cache.ChannelLogos();

        var pn = programmeNames.FirstOrDefault(a => a.DisplayName == User_Tvg_ID);
        if (pn == null)
        {
            return null;
        }

        var channelLogo = channelLogos.FirstOrDefault(a => a.EPGId == pn.Channel);
        if (channelLogo != null)
        {
            return channelLogo.LogoUrl;
        }
        return null;
    }

    public static List<IconFileDto> Icons(this IMemoryCache cache)
    {
        return Get<IconFileDto>(ListIconFiles, cache);
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

    public static IEnumerable<ProgrammeNameDto> ProgrammeNames(this IMemoryCache cache)
    {
        var programmes = cache.Programmes().Where(a => !string.IsNullOrEmpty(a.Channel) && a.StopDateTime > DateTime.Now.AddDays(-1)).ToList();
        if (programmes.Any())
        {
            var ret = programmes.GroupBy(a => a.Channel).Select(group => group.First()).Select(a => new ProgrammeNameDto
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
        throw new Exception($"Cache set Unknown type {data.GetType().Name.ToString()}");
    }

    public static void SetIsSystemReady(this IMemoryCache cache, bool isSystemReady)
    {
        cache.Set(IsSystemReadyKey, isSystemReady, CacheEntryOptions);
    }

    public static List<TvLogoFile> TvLogos(this IMemoryCache cache)
    {
        return Get<TvLogoFile>(ListTVLogos, cache);
    }

    private static List<T> Get<T>(string key, IMemoryCache cache)
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
