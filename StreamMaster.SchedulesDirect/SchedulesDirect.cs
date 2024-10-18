using StreamMaster.Domain.API;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;

using System.Text.Json;

namespace StreamMaster.SchedulesDirect;

public partial class SchedulesDirect(
    ILogger<SchedulesDirect> logger,
    ILogoService logoService,
    IXMLTVBuilder xMLTVBuilder,
    IJobStatusService jobStatusService,
    ISchedulesDirectDataService schedulesDirectDataService,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IOptionsMonitor<SDSettings> _sdSettings,
    IDescriptions descriptions,
    IKeywords keywords,
    ILineupService lineups,
    IProgramService programs,
    IScheduleService schedules,
    ISportsImages sportsImages,
    ISeasonImages seasonImages,
    ISeriesImages seriesImages,
    IMovieImages movieImages) : ISchedulesDirect, IDisposable
{
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly SemaphoreSlim _syncSemaphore = new(1, 1);

    private const int MAX_RETRIES = 3;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(23);

    public static readonly int MaxQueries = 1250;
    public static readonly int MaxDescriptionQueries = 500;
    public static readonly int MaxImgQueries = 125;
    public static readonly int MaxParallelDownloads = 8;

    public async Task<APIResponse> SDSync(CancellationToken cancellationToken)
    {
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

            logger.LogInformation($"DaysToDownload: {_sdSettings.CurrentValue.SDEPGDays}");

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

            bool buildStatus = await lineups.BuildLineupServices(cancellationToken)
                && await schedules.GetAllScheduleEntryMd5S(cancellationToken)
                && await programs.BuildAllProgramEntries(cancellationToken)
                && await descriptions.BuildAllGenericSeriesInfoDescriptions(cancellationToken)
                && keywords.BuildKeywords();

            if (buildStatus)
            {
                await movieImages.GetAllMoviePosters().ConfigureAwait(false);
                await seriesImages.GetAllSeriesImages().ConfigureAwait(false);
                await seasonImages.GetAllSeasonImages().ConfigureAwait(false);
                await sportsImages.GetAllSportsImages().ConfigureAwait(false);

                ClearAllCaches();

                XMLTV? xml = xMLTVBuilder.CreateSDXmlTv("");
                if (xml is not null)
                {
                    WriteXmltv(xml);
                }

                logger.LogInformation("Completed Schedules Direct update execution. SUCCESS.");
                jobManager.SetSuccessful();
                return APIResponse.Ok;
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _ = _syncSemaphore.Release();
        }

        jobManager.SetError();
        return APIResponse.Error;
    }

    private void WriteXmltv(XMLTV xmltv)
    {
        string fileName = Path.Combine(BuildInfo.SDJSONFolder, "streammaster.xmltv");
        if (!FileUtil.WriteXmlFile(xmltv, fileName))
        {
            return;
        }

        FileInfo fileInfo = new(fileName);
        int imageCount = xmltv.Programs.SelectMany(p => p.Icons?.Select(icon => icon.Src) ?? []).Distinct().Count();
        logger.LogInformation($"Completed save of XMLTV file to \"{fileName}\". Size: {FileUtil.BytesToString(fileInfo.Length)}");
        logger.LogDebug($"Generated XMLTV contains {xmltv.Channels.Count} channels, {xmltv.Programs.Count} programs, and {imageCount} distinct image links.");
    }

    public async Task<UserStatus> GetUserStatus(CancellationToken cancellationToken)
    {
        UserStatus? userStatus = await schedulesDirectAPI.GetApiResponse<UserStatus>(APIMethod.GET, "status", cancellationToken: cancellationToken);
        if (userStatus != null)
        {
            logger.LogInformation($"Account expires: {userStatus.Account.Expires:s}Z , Lineups: {userStatus.Lineups.Count}/{userStatus.Account.MaxLineups} , Last update: {userStatus.LastDataUpdate:s}Z");
            TimeSpan expires = userStatus.Account.Expires - SMDT.UtcNow;

            if (expires < TimeSpan.FromDays(7.0))
            {
                logger.LogWarning($"Your Schedules Direct account expires in {expires.Days:D2} days.");
            }

            return userStatus;
        }

        logger.LogError("Did not receive a response from Schedules Direct for a status request.");
        return null!;
    }

    public async Task WriteToCacheAsync<T>(string name, T data, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cachePath = Path.Combine(BuildInfo.SDJSONFolder, SDHelpers.GenerateCacheKey(name));
            SDCacheEntry<T> cacheEntry = new() { Data = data, Command = name, Content = "", Timestamp = SMDT.UtcNow };
            await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(cacheEntry), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    public async Task<T?> GetValidCachedDataAsync<T>(string name, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cachePath = Path.Combine(BuildInfo.SDJSONFolder, SDHelpers.GenerateCacheKey(name));
            if (!File.Exists(cachePath))
            {
                return default;
            }

            string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken).ConfigureAwait(false);
            SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);
            return cacheEntry != null && (DateTime.Now - cacheEntry.Timestamp) <= CacheDuration ? cacheEntry.Data : default;
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

    private void ClearAllCaches()
    {
        lineups.ClearCache();
        schedules.ClearCache();
        programs.ClearCache();
        descriptions.ClearCache();
        movieImages.ClearCache();
        seriesImages.ClearCache();
        seasonImages.ClearCache();
        sportsImages.ClearCache();
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

    public void Dispose()
    {
        _cacheSemaphore.Dispose();
        _syncSemaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}