using System.Diagnostics;
using System.Text.Json;

using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect;

public partial class SchedulesDirect(
    ILogger<SchedulesDirect> logger,
    ILogger<HybridCacheManager<CountryData>> countryDataCacheLogger,
    ILogger<HybridCacheManager<Headend>> HeadendCacheLogger,
    ILogger<HybridCacheManager<LineupPreviewChannel>> lineupPreviewChannelCacheLogger,
    ISDXMLTVBuilder xSDMLTVBuilder,
    IJobStatusService jobStatusService,
    ISchedulesDirectDataService schedulesDirectDataService,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IOptionsMonitor<SDSettings> _sdSettings,
    IMemoryCache memoryCache,
    IDescriptionService descriptions,

    ILineupService lineups,
    IProgramService programs,
    IScheduleService schedules,
    ISportsImages sportsImages,
    ISeasonImages seasonImages,
    ISeriesImages seriesImages,
    IMovieImages movieImages
    ) : ISchedulesDirect, IDisposable
{
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly SemaphoreSlim _syncSemaphore = new(1, 1);

    private const int MAX_RETRIES = 3;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(23);

    private readonly HybridCacheManager<CountryData> CountryDataCache = new(countryDataCacheLogger, memoryCache, defaultKey: "Countries");
    private readonly HybridCacheManager<Headend> HeadendCache = new(HeadendCacheLogger, memoryCache, defaultKey: "Headends", useKeyBasedFiles: true);
    private readonly HybridCacheManager<LineupPreviewChannel> LineupPreviewChannelCache = new(lineupPreviewChannelCacheLogger, memoryCache, useKeyBasedFiles: true);

    public static readonly int MaxQueries = 1250;
    public static readonly int MaxDescriptionQueries = 500;
    public static readonly int MaxImgQueries = 125;
    public static readonly int MaxParallelDownloads = 8;

    public void RemovedExpiredKeys()
    {
        movieImages.RemovedExpiredKeys();
        seasonImages.RemovedExpiredKeys();
        seriesImages.RemovedExpiredKeys();
        sportsImages.RemovedExpiredKeys();
        lineups.RemovedExpiredKeys();
        programs.RemovedExpiredKeys();
        schedules.RemovedExpiredKeys();
        descriptions.RemovedExpiredKeys();
    }

    [LogExecutionTimeAspect]
    public async Task<APIResponse> SDSync(CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();
        JobStatusManager jobManager = jobStatusService.GetJobManageSDSync(EPGHelper.SchedulesDirectId);
        try
        {
            await _syncSemaphore.WaitAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested || !_sdSettings.CurrentValue.SDEnabled)
            {
                jobManager.SetSuccessful();
                return APIResponse.Ok;
            }

            if (jobManager.IsRunning)
            {
                jobManager.SetForceNextRun();
                return APIResponse.Ok;
            }

            jobManager.Start();
            int retryCount = 0;

            while (!CheckToken() && retryCount++ < MAX_RETRIES)
            {
                await Task.Delay(1000, cancellationToken);
            }

            if (!CheckToken())
            {
                jobManager.SetError();
                return APIResponse.ErrorWithMessage("SD Check token errored");
            }

            logger.LogInformation("DaysToDownload: {_sdSettings.CurrentValue.SDEPGDays}", _sdSettings.CurrentValue.SDEPGDays);

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

            bool buildStatus = await lineups.BuildLineupServicesAsync(cancellationToken).ConfigureAwait(false)
                && await schedules.BuildScheduleEntriesAsync(cancellationToken).ConfigureAwait(false)
                && await programs.BuildProgramEntriesAsync(cancellationToken).ConfigureAwait(false)
                && await descriptions.BuildGenericSeriesInfoDescriptionsAsync(cancellationToken).ConfigureAwait(false);
            //&& keywords.BuildKeywords();

            if (buildStatus)
            {
                await movieImages.ProcessArtAsync().ConfigureAwait(false);
                await seriesImages.ProcessArtAsync().ConfigureAwait(false);
                await seasonImages.ProcessArtAsync().ConfigureAwait(false);
                await sportsImages.ProcessArtAsync().ConfigureAwait(false);

                //movieImages.ResetCache();
                //seriesImages.ResetCache();
                //seasonImages.ResetCache();
                //sportsImages.ResetCache();

                XMLTV? xmltv = xSDMLTVBuilder.CreateSDXmlTv();

                if (xmltv is not null)
                {
                    //xmltv.Programs = [xmltv.Programs.First()];

                    string jsonString = JsonSerializer.Serialize(xmltv);
                    FileUtil.WriteJSON(Path.Combine(BuildInfo.AppDataFolder, "sdtv.json"), jsonString);
                    WriteXmltv(xmltv);
                }

                ClearAllCaches();
                schedulesDirectDataService.Reset(EPGHelper.SchedulesDirectId);

                //ResetAllEPGCaches();

                logger.LogInformation("Completed Schedules Direct update execution. SUCCESS.");
                jobManager.SetSuccessful();
                return APIResponse.Ok;
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _ = _syncSemaphore.Release();
            sw.Stop();

            logger.LogInformation("SD Sync took {ms} ms", sw.ElapsedMilliseconds);
        }

        jobManager.SetError();
        return APIResponse.Error;
    }

    private void WriteXmltv(XMLTV xmltv)
    {
        if (!FileUtil.WriteXmlFile(xmltv, BuildInfo.SDXMLFile))
        {
            return;
        }

        FileInfo fileInfo = new(BuildInfo.SDXMLFile);
        int imageCount = xmltv.Programs.SelectMany(p => p.Icons?.Select(icon => icon.Src) ?? []).Distinct().Count();
        logger.LogInformation("Completed save of XMLTV file to \"{BuildInfo.SDXMLFile}\". Size: {FileUtil.BytesToString(fileInfo.Length)}", BuildInfo.SDXMLFile, FileUtil.BytesToString(fileInfo.Length));
        logger.LogDebug("Generated XMLTV contains {xmltv.Channels.Count} channels, {xmltv.Programs.Count} programs, and {imageCount} distinct image links.", xmltv.Channels.Count, xmltv.Programs.Count, imageCount);
    }

    public async Task<UserStatus> GetUserStatus(CancellationToken cancellationToken)
    {
        UserStatus? userStatus = await schedulesDirectAPI.GetApiResponse<UserStatus>(APIMethod.GET, "status", cancellationToken: cancellationToken);
        if (userStatus != null)
        {
            logger.LogInformation(
                "Account expires: {userStatus.Account.Expires:s}Z , Lineups: {userStatus.Lineups.Count}/{userStatus.Account.MaxLineups} , Last update: {userStatus.LastDataUpdate:s}Z",
                userStatus.Account.Expires.ToString("s"),
                userStatus.Lineups.Count,
                userStatus.Account.MaxLineups,
                userStatus.LastDataUpdate.ToString("s")
                );
            TimeSpan expires = userStatus.Account.Expires - SMDT.UtcNow;

            if (expires < TimeSpan.FromDays(7.0))
            {
                logger.LogWarning("Your Schedules Direct account expires in {expires.Days:D2} days.", expires.Days.ToString("D2"));
            }

            return userStatus;
        }

        logger.LogError("Did not receive a response from Schedules Direct for a status request.");
        return null!;
    }

    //public async Task WriteToCacheAsync<T>(string name, T data, CancellationToken cancellationToken = default)
    //{
    //    await _cacheSemaphore.WaitAsync(cancellationToken);
    //    try
    //    {
    //        string cachePath = Path.Combine(BuildInfo.SDJSONFolder, SDHelpers.GenerateCacheKey(name));
    //        SDCacheEntry<T> cacheEntry = new() { Data = data, Command = name, Content = "", Timestamp = SMDT.UtcNow };
    //        await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(cacheEntry), cancellationToken).ConfigureAwait(false);
    //    }
    //    finally
    //    {
    //        _ = _cacheSemaphore.Release();
    //    }
    //}

    //public async Task<T?> GetValidCachedDataAsync<T>(string name, CancellationToken cancellationToken = default)
    //{
    //    await _cacheSemaphore.WaitAsync(cancellationToken);
    //    try
    //    {
    //        string cachePath = Path.Combine(BuildInfo.SDJSONFolder, SDHelpers.GenerateCacheKey(name));
    //        if (!File.Exists(cachePath))
    //        {
    //            return default;
    //        }

    //        string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken).ConfigureAwait(false);
    //        SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);
    //        return cacheEntry != null && (DateTime.Now - cacheEntry.Timestamp) <= CacheDuration ? cacheEntry.Data : default;
    //    }
    //    finally
    //    {
    //        _ = _cacheSemaphore.Release();
    //    }
    //}

    public bool CheckToken(bool forceReset = false)
    {
        return schedulesDirectAPI.CheckToken(forceReset);
    }

    public void ClearAllCaches()
    {
        //lineups.ClearCache();
        //schedules.ClearCache();
        //programs.ClearCache();
        //descriptions.ClearCache();
        //movieImages.ClearCache();
        //seriesImages.ClearCache();
        //seasonImages.ClearCache();
        //sportsImages.ClearCache();
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

    public void ResetAllEPGCaches()
    {
        ClearAllCaches();
        //descriptions.ResetCache();
        //lineups.ResetCache();
        //schedules.ResetCache();
        //programs.ResetCache();
        //movieImages.ResetCache();
        //seriesImages.ResetCache();
        //seasonImages.ResetCache();
        //sportsImages.ResetCache();
        schedulesDirectDataService.Reset(EPGHelper.SchedulesDirectId);
    }

    public void Dispose()
    {
        _cacheSemaphore.Dispose();
        _syncSemaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}