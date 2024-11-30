using System.Text.Json;
using System.Threading.Channels;

using StreamMaster.Domain.Cache;

namespace StreamMaster.SchedulesDirect.Services;

public class ScheduleService(
    ILogger<ScheduleService> logger,
    IHybridCache<ScheduleService> hybridCache,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : IScheduleService, IDisposable
{
    private readonly Channel<(string Md5, string[] Metadata)> scheduleChannel = Channel.CreateUnbounded<(string Md5, string[] Metadata)>();

    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);
    private int cachedSchedules;
    private int downloadedSchedules;
    private readonly int missingGuide;
    private int totalObjects;
    private int processedObjects;

    public async Task<bool> BuildScheduleEntriesAsync(CancellationToken cancellationToken)
    {
        //Dictionary<string, string[]> tempScheduleEntries = new(SchedulesDirect.MaxQueries);
        int days = Math.Clamp(sdSettings.CurrentValue.SDEPGDays, 1, 14);
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        ICollection<MxfService> toProcess = schedulesDirectData.Services.Values;

        logger.LogInformation("Entering BuildScheduleEntriesAsync() for {days} days on {Count} stations.", days, toProcess.Count);
        if (days <= 0)
        {
            logger.LogInformation("Invalid number of days to download. Exiting.");
            return false;
        }

        processedObjects = 0;
        totalObjects = toProcess.Count * days;

        string[] dates = BuildDateArray(days);

        await FillChannelWithScheduleRequestsAsync(toProcess, dates, cancellationToken);
        if (!scheduleChannel.Reader.CanCount || scheduleChannel.Reader.Count == 0)
        {
            logger.LogInformation("No schedules to download. Exiting.");
            return true;
        }

        List<Task> processingTasks = [];
        for (int i = 0; i < SchedulesDirect.MaxParallelDownloads; i++)
        {
            processingTasks.Add(Task.Run(() => FetchAndProcessSchedulesAsync(cancellationToken), cancellationToken));
        }

        await Task.WhenAll(processingTasks).ConfigureAwait(false);

        logger.LogInformation("Processed {processedObjects} entries.", processedObjects);
        return true;
    }

    private async Task FillChannelWithScheduleRequestsAsync(
        ICollection<MxfService> toProcess,
        string[] dates,

        CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(toProcess, cancellationToken, async (mxfService, ct) =>
        {
            foreach (string date in dates)
            {
                string md5 = $"{mxfService.StationId}-{date}";
                ScheduleResponse? schedule = await hybridCache.GetAsync<ScheduleResponse>(md5);
                if (schedule != null)
                {
                    cachedSchedules++;
                    UpdateProgram(schedule);
                }
                else
                {
                    await scheduleChannel.Writer.WriteAsync((md5, [mxfService.StationId, date]), ct).ConfigureAwait(false);
                }
            }
        });

        scheduleChannel.Writer.Complete();
    }

    private async Task FetchAndProcessSchedulesAsync(

        CancellationToken cancellationToken)
    {
        await foreach ((string md5, string[] metadata) in scheduleChannel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            try
            {
                // Fetch schedule from the API
                List<ScheduleResponse>? schedules = await GetScheduleListingsAsync([new ScheduleRequest
                {
                    StationId = metadata[0],
                    Date = [metadata[1]]
                }], cancellationToken).ConfigureAwait(false);

                if (schedules != null)
                {
                    foreach (ScheduleResponse schedule in schedules)
                    {
                        await ProcessSingleScheduleAsync(md5, schedule, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to process schedule for MD5 {md5}: {Error}", md5, ex.Message);
            }
        }
    }

    private async Task ProcessSingleScheduleAsync(string md5, ScheduleResponse schedule, CancellationToken cancellationToken)
    {

        try
        {
            // Cache schedule

            UpdateProgram(schedule);

            string json = JsonSerializer.Serialize(schedule);
            await hybridCache.SetAsync(md5, json).ConfigureAwait(false);

            downloadedSchedules++;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to process single schedule entry for MD5 {md5}: {Error}", md5, ex.Message);
        }
    }

    private void UpdateProgram(ScheduleResponse schedule)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        // Process programs in the schedule
        foreach (ScheduleProgram program in schedule.Programs)
        {
            MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(program.ProgramId);

            BuildScheduleEntry(mxfProgram, program, schedule);
        }
    }

    private void BuildScheduleEntry(MxfProgram mxfProgram, ScheduleProgram program, ScheduleResponse schedule)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        mxfProgram.MD5 = program.Md5;
        MxfScheduleEntry scheduleEntry = new()
        {
            AudioFormat = EncodeAudioFormat(program.AudioProperties),
            Duration = program.Duration,
            StartTime = program.AirDateTime,
            mxfProgram = mxfProgram,
            Is3D = SDHelpers.TableContains(program.VideoProperties, "3d"),
            IsHdtv = SDHelpers.TableContains(program.VideoProperties, "hd"),
            IsPremiere = program.Premiere
        };

        schedulesDirectData.FindOrCreateService(schedule.StationId).MxfScheduleEntries.ScheduleEntry.Add(scheduleEntry);
    }

    private async Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] requests, CancellationToken cancellationToken)
    {
        try
        {
            return await schedulesDirectAPI.GetApiResponse<List<ScheduleResponse>?>(APIMethod.POST, "schedules", requests, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to fetch schedule listings: {Error}", ex.Message);
            return null;
        }
    }

    private static int EncodeAudioFormat(string[] audioProperties)
    {
        return audioProperties == null
            ? 0
            : audioProperties.Contains("dd 5.1", StringComparer.OrdinalIgnoreCase)
            ? 4
            : audioProperties.Contains("dolby", StringComparer.OrdinalIgnoreCase)
            ? 3
            : audioProperties.Contains("stereo", StringComparer.OrdinalIgnoreCase) ? 2 : 0;
    }

    private static string[] BuildDateArray(int days)
    {
        DateTime dt = DateTime.UtcNow;
        string[] dates = new string[days];
        for (int i = 0; i < days; i++)
        {
            dates[i] = dt.ToString("yyyy-MM-dd");
            dt = dt.AddDays(1.0);
        }
        return dates;
    }

    public void Dispose()
    {
        scheduleChannel.Writer.Complete();
        semaphore.Dispose();
    }
    public List<string> GetExpiredKeys()
    {
        return hybridCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null) { }
}
