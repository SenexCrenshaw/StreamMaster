using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;
using StreamMasterDomain.Models;
using StreamMasterDomain.Services;

using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect : ISchedulesDirect
{
    public static readonly int MAX_RETRIES = 3;
    public bool IsSyncing { get; set; }
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly SemaphoreSlim _syncSemaphore = new(1, 1);
    private readonly object fileLock = new();
    private const int MaxQueries = 1250;
    private const int MaxImgQueries = 125;
    public const int MaxParallelDownloads = 8;

    private static int processedObjects;
    private static int totalObjects;
    private static int processStage;

    private readonly ILogger<SchedulesDirect> logger;
    private readonly IEPGCache epgCache;
    private readonly ISchedulesDirectData schedulesDirectData;

    private readonly ISchedulesDirectAPI schedulesDirectAPI;
    private readonly ISettingsService settingsService;
    private readonly IImageDownloadQueue imageDownloadQueue;
    private readonly IMemoryCache memoryCache;

    public SchedulesDirect(ILogger<SchedulesDirect> logger, IImageDownloadQueue imageDownloadQueue, IEPGCache epgCache, ISchedulesDirectData schedulesDirectData, ISchedulesDirectAPI schedulesDirectAPI, ISettingsService settingsService, IMemoryCache memoryCache)
    {
        this.logger = logger;
        this.epgCache = epgCache;
        this.schedulesDirectData = schedulesDirectData;
        this.schedulesDirectAPI = schedulesDirectAPI;
        this.settingsService = settingsService;
        this.memoryCache = memoryCache;
        this.imageDownloadQueue = imageDownloadQueue;
        CheckToken();
    }

    public async Task<bool> SDSync(CancellationToken cancellationToken)
    {
        if (IsSyncing)
        {
            logger.LogWarning("Schedules Direct already Syncing");
            return false;
        }

        try
        {
            await _syncSemaphore.WaitAsync(cancellationToken);

            if (IsSyncing)
            {
                logger.LogWarning("Schedules Direct already Syncing");
                return false;
            }

            IsSyncing = true;
            //if (!await EnsureToken(cancellationToken).ConfigureAwait(false))
            //{
            //    logger.LogWarning("Schedules Direct Token Not Ready");
            //    IsSyncing = false;
            //    return false;
            //}
            //UserStatus status = await GetStatus(cancellationToken);

            //if (!await GetSystemReady(cancellationToken))
            //{
            //    logger.LogWarning("Schedules Direct Not Ready");
            //    IsSyncing = false;
            //    return false;
            //}

            //var startTime = DateTime.UtcNow;
            var setting = memoryCache.GetSetting();
            logger.LogInformation($"DaysToDownload: {setting.SDSettings.SDEPGDays}");

            // load cache file
            epgCache.LoadCache();
            if (
                 await BuildLineupServices(cancellationToken) &&
                    await GetAllScheduleEntryMd5S(setting.SDSettings.SDEPGDays) &&
                    BuildAllProgramEntries() &&
                    BuildAllGenericSeriesInfoDescriptions() &&
                    await GetAllMoviePosters(cancellationToken) &&
                    await GetAllSeriesImages(cancellationToken) &&
                    await GetAllSeasonImages(cancellationToken) &&
                    await GetAllSportsImages(cancellationToken) &&
                    BuildKeywords()
                )
            {
                epgCache.WriteCache();
                CreateDummLineupChannel();
                //var xml = CreateXmltv("");
                //if (xml is not null)
                //{
                //    WriteXmltv(xml);
                //}

                logger.LogInformation("Completed Schedules Direct update execution. SUCCESS.");
                IsSyncing = false;
                return true;
            }
            //StationLogosToDownload = [];
            IsSyncing = false;
            return false;
        }
        finally
        {
            _syncSemaphore.Release();
        }
    }

    private void WriteXmltv(XMLTV xmltv)
    {
        var fileName = Path.Combine(BuildInfo.SDCacheFolder, "epg123.xmltv");
        if (!FileUtil.WriteXmlFile(xmltv, fileName))
        {
            return;
        }

        var fi = new FileInfo(fileName);
        var imageCount = xmltv.Programs.SelectMany(program => program.Icons?.Select(icon => icon.Src) ?? new List<string>()).Distinct().Count();
        logger.LogInformation($"Completed save of the XMLTV file to \"{fileName}\". ({FileUtil.BytesToString(fi.Length)})");
        logger.LogDebug($"Generated XMLTV file contains {xmltv.Channels.Count} channels and {xmltv.Programs.Count} programs with {imageCount} distinct program image links.");
    }

    private void CreateDummLineupChannel()
    {
        var mxfService = schedulesDirectData.FindOrCreateService("DUMMY");
        mxfService.CallSign = "DUMMY";
        mxfService.Name = "DUMMY Station";

        var mxfLineup = schedulesDirectData.FindOrCreateLineup("ZZZ-DUMMY-EPG123", "ZZZ123 Dummy Lineup");
        mxfLineup.channels.Add(new MxfChannel(mxfLineup, mxfService));
    }

    public async Task<UserStatus> GetUserStatus(CancellationToken cancellationToken)
    {
        UserStatus? ret = await schedulesDirectAPI.GetApiResponse<UserStatus>(APIMethod.GET, "status", cancellationToken: cancellationToken);
        if (ret != null)
        {
            logger.LogInformation($"Status request successful. account expires: {ret.Account.Expires:s}Z , lineups: {ret.Lineups.Count}/{ret.Account.MaxLineups} , lastDataUpdate: {ret.LastDataUpdate:s}Z");
            logger.LogInformation($"System status: {ret.SystemStatus[0].Status} , message: {ret.SystemStatus[0].Message}");

            TimeSpan expires = ret.Account.Expires - DateTime.UtcNow;
            if (expires >= TimeSpan.FromDays(7.0))
            {
                return ret;
            }

            logger.LogWarning($"Your Schedules Direct account expires in {expires.Days:D2} days {expires.Hours:D2} hours {expires.Minutes:D2} minutes.");
            logger.LogWarning("*** Renew your Schedules Direct membership at https://schedulesdirect.org. ***");
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for a status request.");
        }

        return ret;

    }

    public void ResetCache(string command)
    {
        string cacheKey = SDHelpers.GenerateCacheKey(command);
        string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);

        if (File.Exists(cachePath))
        {
            File.Delete(cachePath);
        }
    }

    private async Task WriteToCacheAsync<T>(string name, T data, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDJSONFolder, cacheKey);
            SDCacheEntry<T> cacheEntry = new()
            {
                Data = data,
                Command = name,
                Content = "",
                Timestamp = DateTime.UtcNow
            };

            string contentToCache = JsonSerializer.Serialize(cacheEntry);
            await File.WriteAllTextAsync(cachePath, contentToCache, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    private async Task<T?> GetValidCachedDataAsync<T>(string name, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDJSONFolder, cacheKey);
            if (!File.Exists(cachePath))
            {
                return default;
            }

            string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken).ConfigureAwait(false);
            SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);

            return cacheEntry != null && DateTime.Now - cacheEntry.Timestamp <= CacheDuration ? cacheEntry.Data : default;
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    public void CheckToken()
    {
        schedulesDirectAPI.CheckToken();
    }
}