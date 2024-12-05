using System.Diagnostics;
using System.Text.Json;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Models;
namespace StreamMaster.SchedulesDirect;

public partial class SchedulesDirect(
    ILogger<SchedulesDirect> logger,
    ISDXMLTVBuilder xSDMLTVBuilder,
    IDataRefreshService dataRefreshService,
    IFileUtilService fileUtilService,
    IJobStatusService jobStatusService,
    ISchedulesDirectDataService schedulesDirectDataService,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IOptionsMonitor<SDSettings> sdSettings,
    IDescriptionService descriptions,
    ILineupService lineups,
    IProgramService programs,
    IScheduleService schedules,
    ISportsImages sportsImages,
    ISeasonImages seasonImages,
    ISeriesImages seriesImages,
    IMovieImages movieImages,
    IEpisodeImages episodeImages
    ) : IDisposable, ISchedulesDirect
{
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly SemaphoreSlim _syncSemaphore = new(1, 1);

    private const int MAX_RETRIES = 3;

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

            if (cancellationToken.IsCancellationRequested || !sdSettings.CurrentValue.SDEnabled)
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

            while (!await schedulesDirectAPI.ValidateTokenAsync(cancellationToken: cancellationToken) && retryCount++ < MAX_RETRIES)
            {
                await Task.Delay(1000, cancellationToken);
            }

            if (!await schedulesDirectAPI.ValidateTokenAsync(cancellationToken: cancellationToken))
            {
                jobManager.SetError();
                return APIResponse.ErrorWithMessage("SD Check token errored");
            }

            logger.LogInformation("DaysToDownload: {_sdSettings.CurrentValue.SDEPGDays}", sdSettings.CurrentValue.SDEPGDays);

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

            bool buildStatus =
                await lineups.BuildLineupServicesAsync(cancellationToken).ConfigureAwait(false)
                && await schedules.BuildScheduleAndProgramEntriesAsync(cancellationToken).ConfigureAwait(false)
                && await descriptions.BuildGenericSeriesInfoDescriptionsAsync(cancellationToken).ConfigureAwait(false)
                && await programs.BuildProgramEntriesAsync(cancellationToken).ConfigureAwait(false);

            if (buildStatus)
            {
                if (sdSettings.CurrentValue.MovieImages)
                {
                    await movieImages.ProcessArtAsync(cancellationToken).ConfigureAwait(false);
                }

                if (sdSettings.CurrentValue.EpisodeImages)
                {
                    await episodeImages.ProcessArtAsync(cancellationToken).ConfigureAwait(false);
                }

                if (sdSettings.CurrentValue.SeriesImages)
                {
                    await seriesImages.ProcessArtAsync(cancellationToken).ConfigureAwait(false);
                }

                if (sdSettings.CurrentValue.SeasonImages)
                {
                    await seasonImages.ProcessArtAsync(cancellationToken).ConfigureAwait(false);
                }

                if (sdSettings.CurrentValue.SportsImages)
                {
                    await sportsImages.ProcessArtAsync(cancellationToken).ConfigureAwait(false);
                }


                XMLTV? xmltv = xSDMLTVBuilder.CreateSDXmlTv();

                if (xmltv is not null)
                {
                    //xmltv.Programs = [xmltv.Programs.First()];

                    string jsonString = JsonSerializer.Serialize(xmltv);
                    FileUtil.WriteJSON(Path.Combine(BuildInfo.AppDataFolder, "sdtv.json"), jsonString);
                    WriteXmltv(xmltv);

                    List<StationChannelName>? newNames = await fileUtilService.ProcessStationChannelNamesAsync(BuildInfo.SDXMLFile, EPGHelper.SchedulesDirectId, ignoreCache: true);
                    await dataRefreshService.RefreshSchedulesDirect();
                }

                //ClearAllCaches();
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
    public void ResetAllEPGCaches()
    {
        //ClearAllCaches();
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