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
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration= DateTimeOffset.UtcNow.AddMinutes(5) };
    public static readonly string Key = "SDToken";
    public static readonly object Lock = new();
}

internal static class SDUserStatusConfig
{
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5) };
    public static readonly string Key = "SDUserStatus";
    public static readonly object Lock = new();
}

internal static class ChannelLogosConfig
{
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
    public static readonly string Key = "ChannelLogos";
    public static readonly object Lock = new();
}

internal static class TVLogosConfig
{
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
    public static readonly string Key = "TVLogos";
    public static readonly object Lock = new();
}

internal static class ChannelGroupStreamCountsConfig
{
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
    public static readonly string Key = "ChannelGroupStreamCounts";
    public static readonly object Lock = new();
}

internal static class SettingConfig
{
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
    public static readonly string Key = "Setting";
    public static readonly object Lock = new();
}

internal static class IconsConfig
{
    public static readonly MemoryCacheEntryOptions CacheEntryOptions = CacheManagerExtensions.NeverRemoveCacheEntryOptions;
    public static readonly string Key = "Icons";
    public static readonly object Lock = new();
}