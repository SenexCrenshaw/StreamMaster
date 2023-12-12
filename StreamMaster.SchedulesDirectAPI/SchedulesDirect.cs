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
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(23);
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
    public IEPGCache epgCache;
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
        try
        {
            Setting setting = memoryCache.GetSetting();
            if (!setting.SDSettings.SDEnabled)
            {
                memoryCache.SetSyncSuccessful();
                return true;
            }

            if (memoryCache.GetSyncJobStatus().IsRunning)
            {
                memoryCache.SetSyncForceNextRun();
                return false;
            }

            await _syncSemaphore.WaitAsync(cancellationToken);
            memoryCache.GetSyncJobStatus().IsRunning = true;
            bool test = memoryCache.GetSyncJobStatus().IsRunning;
            int maxRetry = 3;
            int retryCount = 0;
            while (!CheckToken() && retryCount++ < maxRetry)
            {
                await Task.Delay(1000, cancellationToken);
            }

            if (!CheckToken())
            {
                memoryCache.SetSyncError();
                return false;
            }


            logger.LogInformation($"DaysToDownload: {setting.SDSettings.SDEPGDays}");

            // load cache file

            epgCache.LoadCache();
            if (
                 await BuildLineupServices(cancellationToken) &&
                    await GetAllScheduleEntryMd5S(cancellationToken) &&
                    BuildAllProgramEntries() &&
                    BuildAllGenericSeriesInfoDescriptions() &&
                    await GetAllMoviePosters(cancellationToken) &&
                    GetAllSeriesImages() &&
                    await GetAllSeasonImages(cancellationToken) &&
                    GetAllSportsImages() &&
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
                memoryCache.SetSyncSuccessful();
                return true;
            }
            //StationLogosToDownload = [];

        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            _syncSemaphore.Release();
        }

        memoryCache.SetSyncError();
        return false;
    }

    private void WriteXmltv(XMLTV xmltv)
    {
        string fileName = Path.Combine(BuildInfo.SDJSONFolder, "epg123.xmltv");
        if (!FileUtil.WriteXmlFile(xmltv, fileName))
        {
            return;
        }

        FileInfo fi = new(fileName);
        int imageCount = xmltv.Programs.SelectMany(program => program.Icons?.Select(icon => icon.Src) ?? new List<string>()).Distinct().Count();
        logger.LogInformation($"Completed save of the XMLTV file to \"{fileName}\". ({FileUtil.BytesToString(fi.Length)})");
        logger.LogDebug($"Generated XMLTV file contains {xmltv.Channels.Count} channels and {xmltv.Programs.Count} programs with {imageCount} distinct program image links.");
    }

    private void CreateDummLineupChannel()
    {
        MxfService mxfService = schedulesDirectData.FindOrCreateService("DUMMY");
        mxfService.CallSign = "DUMMY";
        mxfService.Name = "DUMMY Station";

        MxfLineup mxfLineup = schedulesDirectData.FindOrCreateLineup("ZZZ-DUMMY-EPG123", "ZZZ123 Dummy Lineup");
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
        string cachePath = Path.Combine(BuildInfo.SDJSONFolder, cacheKey);

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
        //return default;
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

            return cacheEntry != null && (DateTime.Now - cacheEntry.Timestamp) <= CacheDuration ? cacheEntry.Data : default;
        }
        catch (Exception ex)
        {
            return default;
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    public bool CheckToken(bool forceReset = false)
    {
        return schedulesDirectAPI.CheckToken(forceReset);
    }

    public void ResetEPGCache()
    {
        ScheduleEntries = [];
        suppressedPrefixes = [];
        seriesDescriptionQueue = [];
        seriesDescriptionResponses = [];
        StationLogosToDownload = [];
        movieImageQueue = [];
        movieImageResponses = [];
        seasonImageQueue = [];
        seasonImageResponses = [];
        sportsImageQueue = [];
        sportsImageResponses = [];
        programQueue = [];
        programResponses = [];
        cachedSchedules = 0;
        processedObjects = 0;
        downloadedSchedules = 0;
        missingGuide = 0;
        seasons = [];
        sportsSeries = [];
        sportEvents = [];
        epgCache.ResetEPGCache();

        schedulesDirectData.ResetLists();
    }
}