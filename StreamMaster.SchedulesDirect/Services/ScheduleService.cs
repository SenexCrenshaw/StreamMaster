using System.Text.Json;

using StreamMaster.Domain.Cache;

namespace StreamMaster.SchedulesDirect.Services;

public class ScheduleService(
    ILogger<ScheduleService> logger,
    IHybridCache<ScheduleService> hybridCache,
    IOptionsMonitor<SDSettings> sdSettings,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : IScheduleService
{
    private int cachedSchedules;
    private int downloadedSchedules;
    private readonly int missingGuide;

    private int totalObjects;
    private int processedObjects;

    public async Task<bool> BuildScheduleEntriesAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, string[]> tempScheduleEntries = new(SchedulesDirect.MaxQueries);

        int days = Math.Clamp(sdSettings.CurrentValue.SDEPGDays, 1, 14);
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        ICollection<MxfService> toProcess = schedulesDirectData.Services.Values;

        logger.LogInformation("Entering GetAllScheduleEntryMd5s() for {days} days on {Count} stations.", days, toProcess.Count);
        if (days <= 0)
        {
            logger.LogInformation("Invalid number of days to download. Exiting.");
            return false;
        }

        processedObjects = 0;
        totalObjects = toProcess.Count * days;

        string[] dates = BuildDateArray(days);

        int batchSize = SchedulesDirect.MaxQueries / days;
        for (int i = 0; i < toProcess.Count; i += batchSize)
        {
            logger.LogInformation("Processing batch {i} of {toProcess.Count} schedules from SD.", i, toProcess.Count);
            bool success = await GetMd5ScheduleEntries(dates, i, tempScheduleEntries, cancellationToken).ConfigureAwait(false);
            if (!success)
            {
                logger.LogError("Problem occurred during GetMd5ScheduleEntries(). Exiting.");
                return false;
            }
        }

        logger.LogInformation("Found {cachedSchedules} cached daily schedules.", cachedSchedules);
        logger.LogInformation("Downloaded {downloadedSchedules} daily schedules.", downloadedSchedules);

        if (CheckMissingGuideData(toProcess.Count))
        {
            return false;
        }

        await ProcessAllSchedulesAsync(tempScheduleEntries, cancellationToken);

        logger.LogInformation("Processed {processedObjects}", processedObjects);
        logger.LogInformation("Exiting GetAllScheduleEntryMd5s(). SUCCESS.");
        return true;
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

    private bool CheckMissingGuideData(int stationCount)
    {
        double missing = (double)missingGuide / stationCount;
        if (missing > 0.1)
        {
            logger.LogError("{missing:N1}% of all stations are missing guide data. Aborting update.", 100 * missing);
            return true;
        }
        return false;
    }

    private async Task<bool> GetMd5ScheduleEntries(string[] dates, int start, Dictionary<string, string[]> tempScheduleEntries, CancellationToken cancellationToken)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<MxfService> toProcess = schedulesDirectData.Services.Values.Skip(start).Take(SchedulesDirect.MaxQueries / dates.Length).ToList();

        if (toProcess.Count == 0)
        {
            return true;
        }

        ScheduleRequest[] requests = BuildScheduleRequests(toProcess, dates);
        Dictionary<string, Dictionary<string, ScheduleMd5Response>>? stationResponses = await GetScheduleMd5sAsync(requests, cancellationToken).ConfigureAwait(false);
        if (stationResponses == null)
        {
            return false;
        }

        List<ScheduleRequest> newRequests = [];
        foreach (ScheduleRequest request in requests)
        {
            string serviceName = request.StationId;
            MxfService mxfService = schedulesDirectData.FindOrCreateService(serviceName);

            if (!stationResponses.TryGetValue(request.StationId, out Dictionary<string, ScheduleMd5Response>? stationResponse))
            {
                HandleMissingStation(request, dates.Length);
                continue;
            }

            List<string> newDateRequests = [];
            await ProcessStationResponseDatesAsync(stationResponse, dates, tempScheduleEntries, mxfService, newDateRequests);

            if (newDateRequests.Count > 0)
            {
                newRequests.Add(new ScheduleRequest
                {
                    StationId = request.StationId,
                    Date = [.. newDateRequests]
                });
            }
        }

        if (newRequests.Count > 0)
        {
            List<ScheduleResponse>? responses = await GetScheduleListingsAsync([.. newRequests], cancellationToken).ConfigureAwait(false);
            if (responses == null)
            {
                return false;
            }

            ProcessScheduleResponses(responses, tempScheduleEntries);
        }

        return true;
    }


    private void HandleMissingStation(ScheduleRequest request, int length)
    {
        logger.LogWarning("Requested stationId {request.StationId} was not present in schedule Md5 response.", request.StationId);
        processedObjects += length;
    }

    private async Task<Dictionary<string, Dictionary<string, ScheduleMd5Response>>?> GetScheduleMd5sAsync(ScheduleRequest[] requests, CancellationToken cancellationToken)
    {
        try
        {
            return await schedulesDirectAPI
                .GetApiResponse<Dictionary<string, Dictionary<string, ScheduleMd5Response>>>(APIMethod.POST, "schedules/md5", requests, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred while fetching MD5 schedule entries: {ex.Message}", ex.Message);
            return null;
        }
    }

    private async Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] requests, CancellationToken cancellationToken)
    {
        try
        {
            return await schedulesDirectAPI
                .GetApiResponse<List<ScheduleResponse>?>(APIMethod.POST, "schedules", requests, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred while fetching daily schedules: {ex.Message}", ex.Message);
            return null;
        }
    }

    private async Task<HashSet<string>> ProcessStationResponseDatesAsync(
       Dictionary<string, ScheduleMd5Response> stationResponse,
       string[] dates,
       Dictionary<string, string[]> tempScheduleEntries,
       MxfService mxfService,
       List<string> newDateRequests)
    {
        HashSet<string> dupeMd5s = [];
        foreach (string day in dates)
        {
            if (stationResponse.TryGetValue(day, out ScheduleMd5Response? dayResponse) && dayResponse.Code == 0 && !string.IsNullOrEmpty(dayResponse.Md5))
            {
                string md5 = dayResponse.Md5;

                if (await hybridCache.ExistsAsync(md5).ConfigureAwait(false))
                {
                    cachedSchedules++;
                }
                else
                {
                    newDateRequests.Add(day);
                }

                if (!tempScheduleEntries.ContainsKey(md5))
                {
                    tempScheduleEntries.Add(md5, [mxfService.StationId, day]);
                }
                else
                {
                    logger.LogWarning("Duplicate schedule Md5 for stationId {mxfService.StationId} on {day}.", mxfService.StationId, day);
                    dupeMd5s.Add(md5);
                }
            }
        }

        return dupeMd5s;
    }

    private void ProcessScheduleResponses(List<ScheduleResponse> responses, Dictionary<string, string[]> tempScheduleEntries)
    {
        foreach (ScheduleResponse response in responses)
        {
            if (response?.Programs == null)
            {
                continue;
            }

            downloadedSchedules++;

            if (tempScheduleEntries.TryGetValue(response.Metadata.Md5, out string[]? serviceDate))
            {
                string jsonString = JsonSerializer.Serialize(response);
                hybridCache.SetAsync(response.Metadata.Md5, jsonString).ConfigureAwait(false);
            }
        }
    }

    private async Task ProcessAllSchedulesAsync(Dictionary<string, string[]> tempScheduleEntries, CancellationToken cancellationToken)
    {
        foreach (KeyValuePair<string, string[]> entry in tempScheduleEntries)
        {
            string md5 = entry.Key;

            if (!await hybridCache.ExistsAsync(md5).ConfigureAwait(false))
            {
                logger.LogInformation("Processing new schedule for MD5: {md5}", md5);

                // Fetch and cache missing data
                string json = JsonSerializer.Serialize(entry.Value);
                await hybridCache.SetAsync(md5, json).ConfigureAwait(false);

                // Process the schedule programs
                ScheduleResponse? schedule = JsonSerializer.Deserialize<ScheduleResponse>(json);
                if (schedule != null)
                {
                    ProcessSchedulePrograms(schedule);
                }
            }

            processedObjects++;
        }
    }

    private static ScheduleRequest[] BuildScheduleRequests(List<MxfService> toProcess, string[] dates)
    {
        return toProcess.Select(service => new ScheduleRequest
        {
            StationId = service.StationId,
            Date = dates
        }).ToArray();
    }

    private void ProcessSchedulePrograms(ScheduleResponse schedule)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        foreach (ScheduleProgram program in schedule.Programs)
        {
            MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(program.ProgramId);

            // Update program properties
            PopulateProgramExtras(mxfProgram, program);

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

            schedulesDirectData.FindOrCreateService(schedule.StationId)
                .MxfScheduleEntries.ScheduleEntry.Add(scheduleEntry);
        }
    }

    private static void PopulateProgramExtras(MxfProgram mxfProgram, ScheduleProgram program)
    {
        if (mxfProgram.Extras.Count == 0)
        {
            mxfProgram.MD5 = program.Md5;
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
    public List<string> GetExpiredKeys()
    {
        return hybridCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null) { }
}
