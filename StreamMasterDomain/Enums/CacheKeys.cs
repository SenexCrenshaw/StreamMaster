using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities.EPG;

namespace StreamMasterDomain.Enums;

public static class CacheKeys
{
    public const string IsSystemReadyKey = "IsSystemReady";

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
        }

        //if (data.GetType().GenericTypeArguments.Contains(typeof(TvLogoFile)))
        //{
        //    cache.Set(ListTVLogos, data, CacheEntryOptions);
        //    return;
        //}

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

    public static List<Programme> Programmes(this IMemoryCache cache)
    {
        return Get<Programme>(ListProgrammes, cache);
    }

    public static void Set(this IMemoryCache cache, object data)
    {
        if (data.GetType().GenericTypeArguments.Contains(typeof(IconFileDto)))
        {
            cache.Set(ListIconFiles, data, CacheEntryOptions);
        }

        if (data.GetType().GenericTypeArguments.Contains(typeof(TvLogoFile)))
        {
            cache.Set(ListTVLogos, data, CacheEntryOptions);
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
