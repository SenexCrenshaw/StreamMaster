using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Models;
using StreamMaster.SchedulesDirect.Domain.Enums;
using StreamMaster.SchedulesDirect.Helpers;

using System.Text.Json;

namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirect(
    ILogger<SchedulesDirect> logger,
    IIconService iconService,
    IXMLTVBuilder xMLTVBuilder,
    IJobStatusService jobStatusService,
    ISchedulesDirectDataService schedulesDirectDataService,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IMemoryCache memoryCache,
    IDescriptions descriptions,
    IKeywords keywords,
    ILineups lineups,
    IPrograms programs,
    ISchedules schedules,
    ISportsImages sportsImages,
        ISeasonImages seasonImages,
            ISeriesImages seriesImages,
            IMovieImages movieImages
) : ISchedulesDirect
{
    public static readonly int MAX_RETRIES = 3;
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(23);
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly SemaphoreSlim _syncSemaphore = new(1, 1);


    public static readonly int MaxQueries = 1250;
    public static readonly int MaxDescriptionQueries = 500;
    public static readonly int MaxImgQueries = 125;
    public static readonly int MaxParallelDownloads = 8;

    public async Task<bool> SDSync(int EPGNumber, CancellationToken cancellationToken)
    {

        try
        {
            await _syncSemaphore.WaitAsync(cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                jobStatusService.SetSyncSuccessful();
                return false;
            }

            Setting setting = memoryCache.GetSetting();
            if (!setting.SDSettings.SDEnabled)
            {
                jobStatusService.SetSyncSuccessful();
                return true;
            }

            if (jobStatusService.GetSyncJobStatus().IsRunning)
            {
                jobStatusService.SetSyncForceNextRun();
                return false;
            }

            jobStatusService.SetSyncIsRunning(true);
            //ResetEPGCache();
            int maxRetry = 3;
            int retryCount = 0;
            while (!CheckToken() && retryCount++ < maxRetry)
            {
                await Task.Delay(1000, cancellationToken);
            }

            if (!CheckToken())
            {
                jobStatusService.SetSyncError();
                return false;
            }


            logger.LogInformation($"DaysToDownload: {setting.SDSettings.SDEPGDays}");

            // load cache file
            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

            if (

            await lineups.BuildLineupServices(cancellationToken) &&
                    await schedules.GetAllScheduleEntryMd5S(cancellationToken) &&
                    await programs.BuildAllProgramEntries(cancellationToken) &&
                    await descriptions.BuildAllGenericSeriesInfoDescriptions() &&
                    keywords.BuildKeywords()
                )
            {
                await movieImages.GetAllMoviePosters().ConfigureAwait(false);
                await seriesImages.GetAllSeriesImages().ConfigureAwait(false);
                await seasonImages.GetAllSeasonImages().ConfigureAwait(false);
                await sportsImages.GetAllSportsImages().ConfigureAwait(false);

                lineups.ClearCache();
                schedules.ClearCache();
                programs.ClearCache();
                descriptions.ClearCache();

                movieImages.ClearCache();
                seriesImages.ClearCache();
                seasonImages.ClearCache();
                sportsImages.ClearCache();

                HandleDummies();

                XMLTV? xml = xMLTVBuilder.CreateSDXmlTv("");
                if (xml is not null)
                {
                    WriteXmltv(xml);
                }

                logger.LogInformation("Completed Schedules Direct update execution. SUCCESS.");
                jobStatusService.SetSyncSuccessful();
                return true;
            }
            //StationLogosToDownload = [];

        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            _ = _syncSemaphore.Release();
        }

        jobStatusService.SetSyncError();
        return false;
    }

    private void WriteXmltv(XMLTV xmltv)
    {
        string fileName = Path.Combine(BuildInfo.SDJSONFolder, "streammaster.xmltv");
        if (!FileUtil.WriteXmlFile(xmltv, fileName))
        {
            return;
        }

        FileInfo fi = new(fileName);
        int imageCount = xmltv.Programs.SelectMany(program => program.Icons?.Select(icon => icon.Src) ?? []).Distinct().Count();
        logger.LogInformation($"Completed save of the XMLTV file to \"{fileName}\". ({FileUtil.BytesToString(fi.Length)})");
        logger.LogDebug($"Generated XMLTV file contains {xmltv.Channels.Count} channels and {xmltv.Programs.Count} programs with {imageCount} distinct program image links.");
    }

    private void HandleDummies()
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        MxfService mxfService = schedulesDirectData.FindOrCreateService("DUMMY");
        mxfService.CallSign = "DUMMY";
        mxfService.Name = "DUMMY Station";

        MxfLineup mxfLineup = schedulesDirectData.FindOrCreateLineup("ZZZ-DUMMY-StreamMaster", "ZZZSM Dummy Lineup");
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
        descriptions.ResetCache();
        lineups.ResetCache();
        schedules.ResetCache();
        programs.ResetCache();

        movieImages.ResetCache();
        seriesImages.ResetCache();
        seasonImages.ResetCache();
        sportsImages.ResetCache();

        schedulesDirectDataService.Reset(EPGHelper.SchedulesDirectId);
    }
}