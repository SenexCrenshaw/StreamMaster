using System.Collections.Concurrent;
using System.Text.Json;

using X.PagedList.Extensions;
namespace StreamMaster.SchedulesDirect.Services;

public class ScheduleService(ILogger<ScheduleService> logger, IImageDownloadService imageDownloadService, IImageDownloadQueue imageDownloadQueue, IOptionsMonitor<SDSettings> sdSettings, ISchedulesDirectAPIService schedulesDirectAPI, IEPGCache<ScheduleService> epgCache, ISchedulesDirectDataService schedulesDirectDataService)
    : IScheduleService
{
    private int cachedSchedules;
    private int downloadedSchedules;
    private int missingGuide;
    private int totalObjects;
    private int processedObjects;

    public async Task<bool> GetAllScheduleEntryMd5S(CancellationToken cancellationToken)
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

        ProcessAllSchedules(tempScheduleEntries);
        epgCache.SaveCache();
        logger.LogInformation("Processed {processedObjects}", processedObjects);
        logger.LogInformation("Exiting GetAllScheduleEntryMd5s(). SUCCESS.");
        return true;
    }

    private static string[] BuildDateArray(int days)
    {
        DateTime dt = SMDT.UtcNow;
        string[] dates = new string[days];
        for (int i = 0; i < dates.Length; ++i)
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
            Dictionary<int, string> requestErrors = [];
            string serviceName = request.StationId;// $"{EPGHelper.SchedulesDirectId}-{request.StationId}";
            MxfService mxfService = schedulesDirectData.FindOrCreateService(serviceName);

            if (!stationResponses.TryGetValue(request.StationId, out Dictionary<string, ScheduleMd5Response>? stationResponse))
            {
                // Log and handle missing stations here
                HandleMissingStation(request, dates.Length);
                continue;
            }

            List<string> newDateRequests = [];
            ProcessStationResponseDates(stationResponse, dates, tempScheduleEntries, mxfService, newDateRequests);

            // Create new requests for any missing dates
            if (newDateRequests.Count > 0)
            {
                newRequests.Add(new ScheduleRequest
                {
                    StationId = request.StationId,
                    Date = [.. newDateRequests]
                });
            }

            // Log and handle request errors
            foreach (KeyValuePair<int, string> keyValuePair in requestErrors)
            {
                logger.LogWarning("Requests for MD5 schedule entries of station {request.StationId} returned error code {keyValuePair.Key}, message: {keyValuePair.Value}", request.StationId, keyValuePair.Key, keyValuePair.Value);
            }
        }

        if (newRequests.Count > 0)
        {
            List<ScheduleResponse>? responses = await GetScheduleListingsAsync([.. newRequests], cancellationToken).ConfigureAwait(false);
            if (responses == null)
            {
                return false;
            }

            //ScheduleProgram? test = responses.SelectMany(a => a.Programs.Where(a => a.New && a.Multipart.TotalParts > 0)).FirstOrDefault();
            //if (test != null)
            //{

            //    LogoInfo nl = new("image/" + test.ProgramId);
            //    bool a = await imageDownloadService.DownloadImageAsync(nl, cancellationToken);

            //}

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
        DateTime dtStart = DateTime.Now;

        try
        {
            Dictionary<string, Dictionary<string, ScheduleMd5Response>>? result = await schedulesDirectAPI
                .GetApiResponse<Dictionary<string, Dictionary<string, ScheduleMd5Response>>>(APIMethod.POST, "schedules/md5", requests, cancellationToken)
                .ConfigureAwait(false);

            if (result != null)
            {
                logger.LogDebug("Successfully retrieved MD5s for {result.Count}/{requests.Length} stations' daily schedules. Time taken: {duration}", result.Count, requests.Length, (DateTime.Now - dtStart).ToString("G"));
            }
            else
            {
                logger.LogError("Did not receive a response from Schedules Direct for MD5s of {requests.Length} stations' daily schedules. Time taken: {duration}", requests.Length, (DateTime.Now - dtStart).ToString("G"));
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred while fetching MD5 schedule entries: {ex.Message}", ex.Message);
            return null;
        }
    }

    private async Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] requests, CancellationToken cancellationToken)
    {
        DateTime dtStart = DateTime.Now;

        try
        {
            List<ScheduleResponse>? result = await schedulesDirectAPI
                .GetApiResponse<List<ScheduleResponse>?>(APIMethod.POST, "schedules", requests, cancellationToken)
                .ConfigureAwait(false);

            if (result != null)
            {
                logger.LogDebug("Successfully retrieved {requests.Length} stations' daily schedules. Time taken: {duration}", requests.Length, (DateTime.Now - dtStart).ToString("G"));
            }
            else
            {
                logger.LogError("Did not receive a response from Schedules Direct for {requests.Length} stations' daily schedules. Time taken: {DateTime.Now - dtStart:G}", requests.Length, (DateTime.Now - dtStart).ToString("G"));
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred while fetching daily schedules: {ex.Message}", ex.Message);
            return null;
        }
    }

    private static ScheduleRequest[] BuildScheduleRequests(List<MxfService> toProcess, string[] dates)
    {
        return toProcess.Select(service =>
        {
            //(int epgNumber, string stationId) = ePGHelper.ExtractEPGNumberAndStationId(service.StationId);
            return new ScheduleRequest { StationId = service.StationId, Date = dates };
        }).ToArray();
    }
    private HashSet<string> ProcessStationResponseDates(Dictionary<string, ScheduleMd5Response> stationResponse, string[] dates, Dictionary<string, string[]> tempScheduleEntries, MxfService mxfService, List<string> newDateRequests)
    {
        HashSet<string> dupeMd5s = [];
        foreach (string day in dates)
        {
            if (stationResponse.TryGetValue(day, out ScheduleMd5Response? dayResponse) && dayResponse.Code == 0 && !string.IsNullOrEmpty(dayResponse.Md5))
            {
                if (epgCache.JsonFiles.ContainsKey(dayResponse.Md5))
                {
                    ++processedObjects;
                    ++cachedSchedules;
                }
                else
                {
                    newDateRequests.Add(day);
                }

                if (!tempScheduleEntries.TryGetValue(dayResponse.Md5, out string[]? value))
                {
                    tempScheduleEntries.Add(dayResponse.Md5, [mxfService.StationId, day]);
                }
                else
                {
                    logger.LogWarning("Duplicate schedule Md5 for stationId {mxfService.StationId} on {day} with {value[1]}.", mxfService.StationId, day, value[1]);
                    dupeMd5s.Add(dayResponse.Md5);
                }
            }
        }

        return dupeMd5s;
    }

    private void ProcessScheduleResponses(List<ScheduleResponse> responses, Dictionary<string, string[]> tempScheduleEntries)
    {

        foreach (ScheduleResponse response in responses)
        {
            ++processedObjects;
            if (response?.Programs == null)
            {
                continue;
            }

            ++downloadedSchedules;

            if (tempScheduleEntries.TryGetValue(response.Metadata.Md5, out string[]? serviceDate))
            {
                try
                {

                    string jsonString = JsonSerializer.Serialize(response, BuildInfo.JsonIndentOptionsWhenWritingNull);
                    epgCache.AddAsset(response.Metadata.Md5, jsonString);
                }
                catch (Exception ex)
                {
                    logger.LogInformation("Failed to write station daily schedule to cache. Station: {serviceDate[0]}, Date: {serviceDate[1]}. Exception: {FileUtil.ReportExceptionMessages(ex)}", serviceDate[0], serviceDate[1], FileUtil.ReportExceptionMessages(ex));
                }
            }
        }
    }

    private void ProcessAllSchedules(Dictionary<string, string[]> tempScheduleEntries)
    {
        foreach (string md5 in tempScheduleEntries.Keys)
        {
            ProcessMd5ScheduleEntry(md5);
        }

        logger.LogInformation("Processed {totalObjects} daily schedules for {Count} stations for an average of {totalObjects:N1} days per station.", totalObjects, tempScheduleEntries.Count, (double)totalObjects / tempScheduleEntries.Count);
    }

    private void ProcessMd5ScheduleEntry(string md5)
    {

        if (!epgCache.JsonFiles.ContainsKey(md5))
        {
            return;
        }

        try
        {
            string? asset = epgCache.GetAsset(md5);
            if (asset != null)
            {
                ScheduleResponse? schedule = JsonSerializer.Deserialize<ScheduleResponse>(asset, BuildInfo.JsonIndentOptionsWhenWritingNull);
                if (schedule != null)
                {
                    ProcessSchedulePrograms(schedule);
                }
                else
                {
                    logger.LogError("Failed to read Md5Schedule entry in cache file.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred when trying to read Md5Schedule entry in cache file. Exception: {FileUtilException}", FileUtil.ReportExceptionMessages(ex));
        }
    }

    private void ProcessSchedulePrograms(ScheduleResponse schedule)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        //List<ScheduleProgram> a = schedule.Programs.Where(a => a.Multipart != null).ToList();

        //List<ScheduleProgram> b = a.Where(a => a.Multipart.TotalParts > 0).ToList();
        if (schedule.Programs.Count > 0)
        {
            int aaa = 1;
        }
        else
        {
            return;
        }

        // Process each program in the schedule
        foreach (ScheduleProgram scheduleProgram in schedule.Programs)
        {
            // Skip programs that have already aired
            //if (scheduleProgram.AirDateTime + TimeSpan.FromSeconds(scheduleProgram.Duration) < SMDT.UtcNow)
            //{
            //    continue;
            //}
            ConcurrentBag<ProgramMetadata> metadata = [];

            //// Find or create a program in the MXF service based on the ProgramId
            MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(scheduleProgram.ProgramId);

            //// Populate program Extras and multipart details
            PopulateProgramExtras(mxfProgram, scheduleProgram);

            // Create the schedule entry and populate fields
            MxfScheduleEntry scheduleEntry = new()
            {
                AudioFormat = EncodeAudioFormat(scheduleProgram.AudioProperties),
                Duration = scheduleProgram.Duration,
                StartTime = scheduleProgram.AirDateTime,
                mxfProgram = mxfProgram,

                // Direct property assignments using helper methods to simplify:
                Is3D = SDHelpers.TableContains(scheduleProgram.VideoProperties, "3d"),
                IsCc = SDHelpers.TableContains(scheduleProgram.AudioProperties, "cc"),
                IsDvs = SDHelpers.TableContains(scheduleProgram.AudioProperties, "dvs"),
                IsHdtv = SDHelpers.TableContains(scheduleProgram.VideoProperties, "hd"),
                IsSap = SDHelpers.TableContains(scheduleProgram.AudioProperties, "sap"),
                IsSubtitled = SDHelpers.TableContains(scheduleProgram.AudioProperties, "subtitled"),
                IsBlackout = scheduleProgram.SubjectToBlackout,
                IsClassroom = scheduleProgram.CableInTheClassroom,
                IsInProgress = scheduleProgram.JoinedInProgress,
                IsPremiere = scheduleProgram.Premiere || scheduleProgram.IsPremiereOrFinale.StringContains("premiere"),
                IsRepeat = !scheduleProgram.New,
                IsFinale = scheduleProgram.IsPremiereOrFinale.StringContains("finale"),
                IsSigned = scheduleProgram.Signed,
                Part = scheduleProgram.Multipart?.PartNumber ?? 0,
                Parts = scheduleProgram.Multipart?.TotalParts ?? 0
            };

            // Add the ratings to the schedule entry

            if (mxfProgram.Extras.TryGetValue("ratings", out dynamic? ratings))
            {
                if (ratings is List<XmltvRating>)
                {
                    scheduleEntry.extras.Add("ratings", ratings);
                }
                else
                {
                    int aa = 1;
                }
            }

            //if (scheduleProgram.Multipart.TotalParts > 0)
            //{
            //    mxfProgram.mxfSeason = schedulesDirectData.FindOrCreateSeason(mxfProgram.mxfSeriesInfo.SeriesId, mxfProgram.SeasonNumber, mxfProgram.ProgramId);
            //}


            MxfService mxfService = schedulesDirectData.FindOrCreateService(schedule.StationId);
            //XmltvProgramme prog = xmltvProgramBuilder.BuildXmltvProgram(scheduleEntry, schedule.StationId, 0, "");
            //mxfService.Programmes.Add(prog);
            mxfService.MxfScheduleEntries.ScheduleEntry.Add(scheduleEntry);
        }
    }


    private static void PopulateProgramExtras(MxfProgram mxfProgram, ScheduleProgram scheduleProgram)
    {
        if (mxfProgram.Extras.Count == 0)
        {
            //mxfProgram.UidOverride = $"{scheduleProgram.ProgramId[..10]}_{scheduleProgram.ProgramId[10..]}";
            mxfProgram.MD5 = scheduleProgram.Md5;
            //mxfProgram.Extras.Add("md5", scheduleProgram.Md5);
            if (scheduleProgram.Multipart?.PartNumber > 0)
            {
                mxfProgram.Extras.Add("multipart", $"{scheduleProgram.Multipart.PartNumber}/{scheduleProgram.Multipart.TotalParts}");
            }
        }

        // Use the existing methods to populate flags and ratings
        PopulateProgramFlags(mxfProgram, scheduleProgram);
        PopulateProgramRatings(mxfProgram, scheduleProgram);
    }

    private static int EncodeAudioFormat(string[] audioProperties)
    {
        int maxValue = 0;
        if (audioProperties == null)
        {
            return maxValue;
        }

        foreach (string property in audioProperties)
        {
            switch (property.ToLower())
            {
                case "stereo":
                    maxValue = Math.Max(maxValue, 2);
                    break;

                case "dolby":
                case "surround":
                    maxValue = Math.Max(maxValue, 3);
                    break;

                case "dd":
                case "dd 5.1":
                    maxValue = Math.Max(maxValue, 4);
                    break;
            }
        }
        return maxValue;
    }

    private static void PopulateProgramFlags(MxfProgram mxfProgram, ScheduleProgram scheduleProgram)
    {
        mxfProgram.IsSeasonFinale |= scheduleProgram.IsPremiereOrFinale.StringContains("Season Finale");
        mxfProgram.IsSeasonPremiere |= scheduleProgram.IsPremiereOrFinale.StringContains("Season Premiere");
        mxfProgram.IsSeriesFinale |= scheduleProgram.IsPremiereOrFinale.StringContains("Series Finale");
        mxfProgram.IsSeriesPremiere |= scheduleProgram.IsPremiereOrFinale.StringContains("Series Premiere");

        if (!mxfProgram.Extras.ContainsKey("premiere"))
        {
            mxfProgram.Extras.Add("premiere", false);
        }

        if (scheduleProgram.Premiere)
        {
            mxfProgram.Extras.AddOrUpdate("premiere", true);
        }
    }

    private static void PopulateProgramRatings(MxfProgram mxfProgram, ScheduleProgram scheduleProgram)
    {
        Dictionary<string, string> scheduleTvRatings = [];

        if (scheduleProgram.Ratings != null)
        {
            foreach (ScheduleTvRating rating in scheduleProgram.Ratings)
            {
                scheduleTvRatings.Add(rating.Body, rating.Code);
            }
        }

        if (!mxfProgram.Extras.TryGetValue("ratings", out dynamic? value))
        {
            mxfProgram.Extras.Add("ratings", scheduleTvRatings);
        }
        else if (scheduleTvRatings.Count > 0)
        {
            Dictionary<string, string> existingRatings = value;
            foreach (KeyValuePair<string, string> rating in scheduleTvRatings)
            {
                if (!existingRatings.ContainsKey(rating.Key))
                {
                    existingRatings.Add(rating.Key, rating.Value);
                }
            }

            mxfProgram.Extras["ratings"] = existingRatings;
        }
    }

    public void ClearCache()
    {
        cachedSchedules = 0;
        downloadedSchedules = 0;
        missingGuide = 0;
    }

    public void ResetCache()
    {
        epgCache.ResetCache();
    }

    public List<string> GetExpiredKeys()
    {
        return epgCache.GetExpiredKeys();
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null)
    {
        epgCache.RemovedExpiredKeys(keysToDelete);
    }
}