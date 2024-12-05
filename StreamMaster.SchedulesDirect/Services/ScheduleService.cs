using System.Collections.Concurrent;
using System.Text.Json;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Domain;

namespace StreamMaster.SchedulesDirect.Services;

public class ScheduleService(
    ILogger<ScheduleService> logger,
    SMCacheManager<ScheduleService> hybridCache,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    IProgramRepository programRepository,
    ISchedulesDirectDataService schedulesDirectDataService
    ) : IScheduleService, IDisposable
{
    private readonly ConcurrentDictionary<string, string[]> schedulesToProcess = new();
    private readonly SemaphoreSlim apiSemaphore = new(SDAPIConfig.MaxParallelDownloads);
    private readonly SemaphoreSlim writeSema = new(1, 1);


    private int cachedSchedules;
    private int downloadedSchedules;
    private readonly int missingGuide;
    private int totalObjects;
    private int processedObjects;

    public async Task<bool> BuildScheduleAndProgramEntriesAsync(CancellationToken cancellationToken)
    {
        //Dictionary<string, string[]> tempScheduleEntries = new(SchedulesDirect.MaxQueries);
        int days = Math.Clamp(sdSettings.CurrentValue.SDEPGDays, 1, 14);
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;
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
        if (schedulesToProcess.IsEmpty)
        {
            logger.LogInformation("No schedules to download. Exiting.");
            return true;
        }

        int threads = Math.Clamp(schedulesToProcess.Count, 1, SDAPIConfig.MaxParallelDownloads);

        List<Task> processingTasks = [];
        for (int i = 0; i < threads; i++)
        {
            processingTasks.Add(Task.Run(() => FetchAndProcessSchedulesAsync(cancellationToken), cancellationToken));
        }

        await Task.WhenAll(processingTasks).ConfigureAwait(false);

        logger.LogInformation("Processed {processedObjects} entries.", processedObjects);
        return true;
    }

    private async Task FillChannelWithScheduleRequestsAsync(ICollection<MxfService> toProcess, string[] dates, CancellationToken cancellationToken)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

        await Parallel.ForEachAsync(toProcess, cancellationToken, async (mxfService, _) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (string date in dates)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string key = $"{mxfService.StationId}-{date}";
                ScheduleResponse? schedule = await hybridCache.GetAsync<ScheduleResponse>(key);
                if (schedule != null)
                {
                    cachedSchedules++;
                    await UpdateProgramAsync(schedule, cancellationToken);
                }
                else
                {
                    schedulesToProcess.TryAdd(key, [mxfService.StationId, date]);
                }
            }
        });

    }

    private async Task FetchAndProcessSchedulesAsync(CancellationToken cancellationToken)
    {
        const int batchSize = 100; // Number of schedule requests per batch

        //while (!cancellationToken.IsCancellationRequested)
        //{
        List<KeyValuePair<string, string[]>> currentBatch = new(batchSize);
        //string[] keys = schedulesToProcess.Keys.ToArray(); 

        foreach (string md5 in schedulesToProcess.Keys)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (schedulesToProcess.TryRemove(md5, out string[]? metadata))
            {
                currentBatch.Add(new KeyValuePair<string, string[]>(md5, metadata));

                // Process the batch if the size limit is reached
                if (currentBatch.Count >= batchSize)
                {
                    await ProcessAndClearBatchAsync(currentBatch, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        // Process any remaining items in the last batch
        if (currentBatch.Count > 0)
        {
            await ProcessAndClearBatchAsync(currentBatch, cancellationToken).ConfigureAwait(false);
        }

        await Task.Delay(10, cancellationToken).ConfigureAwait(false); // Pause to allow new additions to schedulesToProcess
                                                                       // }
    }

    private async Task ProcessAndClearBatchAsync(List<KeyValuePair<string, string[]>> batch, CancellationToken cancellationToken)
    {
        try
        {
            await ProcessScheduleBatchAsync(batch, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process schedule batch. Retrying items...");
        }
        finally
        {
            batch.Clear(); // Clear the batch in any case
        }
    }

    private async Task ProcessScheduleBatchAsync(List<KeyValuePair<string, string[]>> batch, CancellationToken cancellationToken)
    {
        await apiSemaphore.WaitAsync(cancellationToken); // Ensure the semaphore limits concurrent API calls
        try
        {
            // Create batch requests
            ScheduleRequest[] scheduleRequests = batch.Select(kv => new ScheduleRequest
            {
                StationId = kv.Value[0],
                Date = [kv.Value[1]]
            }).ToArray();

            // Fetch scheduleResponses in a single API call
            List<ScheduleResponse>? scheduleResponses = await schedulesDirectAPI
                .GetScheduleListingsAsync(scheduleRequests, cancellationToken)
                .ConfigureAwait(false);

            if (scheduleResponses != null)
            {
                await ProcessSchedulesAsync(scheduleResponses, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing schedule batch of {BatchSize} requests.", batch.Count);
            throw; // Rethrow to trigger retry in FetchAndProcessSchedulesAsync
        }
        finally
        {
            apiSemaphore.Release();
        }
    }

    private async Task ProcessSchedulesAsync(List<ScheduleResponse> schedules, CancellationToken cancellationToken)
    {
        Dictionary<string, string> bulkItems = [];

        foreach (ScheduleResponse schedule in schedules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string key = $"{schedule.StationId}-{schedule.Metadata.StartDate}";
            try
            {

                await UpdateProgramAsync(schedule, cancellationToken);

                //string json = JsonSerializer.Serialize(schedule);
                bulkItems[key] = JsonSerializer.Serialize(schedule);
                //await hybridCache.SetAsync(md5, json).ConfigureAwait(false);

                Interlocked.Increment(ref downloadedSchedules);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process single schedule entry for MD5 {Md5}.", key);
            }
        }

        if (bulkItems.Count > 0)
        {
            await writeSema.WaitAsync(cancellationToken);

            try
            {
                await hybridCache.SetBulkAsync(bulkItems).ConfigureAwait(false);
            }
            finally
            {
                writeSema.Release();
            }

        }
    }

    private async Task UpdateProgramAsync(ScheduleResponse schedule, CancellationToken cancellationToken)
    {
        if (schedule.StationId.Contains("68827"))
        {
            int aaa = 1;
        }
        // Process programs in the schedule
        foreach (ScheduleProgram program in schedule.Programs)
        {

            cancellationToken.ThrowIfCancellationRequested();
            MxfProgram? mxfProgram = await programRepository.FindOrCreateProgram(program.ProgramId, program.Md5);

            if (mxfProgram == null)
            {
                logger.LogWarning("Program {ProgramId} not found in the data store.", program.ProgramId);
                continue;
            }
            BuildScheduleEntry(mxfProgram, program, schedule);
        }
    }

    private void BuildScheduleEntry(MxfProgram mxfProgram, ScheduleProgram program, ScheduleResponse schedule)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

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
        schedulesToProcess.Clear();
        apiSemaphore.Dispose();
    }
    public List<string> GetExpiredKeys()
    {
        return hybridCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null) { }
}
