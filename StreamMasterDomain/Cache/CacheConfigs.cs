using Microsoft.Extensions.Caching.Memory;

namespace StreamMasterDomain.Cache;

internal static class IsSystemReadyConfig
{
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
    public static readonly string Key = "IsSystemReady";
    public static readonly object Lock = new();
}

internal static class SDTokenConfig
{
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration= DateTimeOffset.UtcNow.AddHours(1) };
    public static readonly string Key = "SDToken";
    public static readonly object Lock = new();
}

//internal static class GuideImageConfig
//{
//    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
//    public static readonly string Key = "ListGuideImages";
//    public static readonly object Lock = new();
//}

//internal static class LineupConfig
//{
//    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
//    public static readonly string Key = "ListLineUps";
//    public static readonly object Lock = new();
//}

//internal static class StationConfig
//{
//    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
//    public static readonly string Key = "ListStations";
//    public static readonly object Lock = new();
//}