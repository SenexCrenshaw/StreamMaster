using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Common;
using StreamMaster.SchedulesDirect.Domain.Enums;
using StreamMaster.SchedulesDirect.Helpers;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StreamMaster.SchedulesDirect;

public class Schedules(ILogger<Schedules> logger, IMemoryCache memoryCache, ISchedulesDirectAPIService schedulesDirectAPI, IEPGCache<Schedules> epgCache, ISchedulesDirectDataService schedulesDirectDataService) : ISchedules
{
    private int cachedSchedules;
    private int downloadedSchedules;

    private int missingGuide;
    private int processedObjects;
    private int totalObjects;
    private int processStage;

    public async Task<bool> GetAllScheduleEntryMd5S(CancellationToken cancellationToken)
    {
        Dictionary<string, string[]> tempScheduleEntries = [];
        Setting settings = memoryCache.GetSetting();
        int days = settings.SDSettings.SDEPGDays;
        days = Math.Clamp(days, 1, 14);
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        ICollection<MxfService> toProcess = schedulesDirectData.Services.Values;

        IEnumerable<string> duplicateStationIds = toProcess
    .GroupBy(service => service.StationId)
    .Where(group => group.Count() > 1)
    .Select(group => group.Key);

        logger.LogInformation("Entering GetAllScheduleEntryMd5s() for {days} days on {Count} stations.", days, toProcess.Count);
        if (days <= 0)
        {
            logger.LogInformation("Invalid number of days to download. Exiting.");
            return false;
        }

        // populate station prefixes to suppress
        // suppressedPrefixes = new List<string>(config.SuppressStationEmptyWarnings.Split(','));

        // reset counter
        processedObjects = 0;
        totalObjects = toProcess.Count * days;
        //++processStage;

        // build date array for requests
        DateTime dt = DateTime.UtcNow;

        // build the date array to request
        string[] dates = new string[days];
        for (int i = 0; i < dates.Length; ++i)
        {
            dates[i] = dt.ToString("yyyy-MM-dd");
            dt = dt.AddDays(1.0);
        }


        int toAdd = SchedulesDirect.MaxQueries / dates.Length;
        for (int i = 0; i < toProcess.Count; i += SchedulesDirect.MaxQueries / dates.Length)
        {
            logger.LogInformation($"Getting {i} of {toProcess.Count} schedules from SD.");

            if (await GetMd5ScheduleEntries(dates, i, tempScheduleEntries, cancellationToken))
            {
                continue;
            }

            logger.LogError("Problem occurred during GetMd5ScheduleEntries(). Exiting.");
            return false;
        }
        logger.LogInformation("Found {cachedSchedules} cached daily schedules.", cachedSchedules);
        logger.LogInformation("Downloaded {downloadedSchedules} daily schedules.", downloadedSchedules);

        double missing = (double)missingGuide / toProcess.Count;
        if (missing > 0.1)
        {
            logger.LogError("{missing:N1}% of all stations are missing guide data. Aborting update.", 100 * missing);
            return false;
        }

        // reset counters again
        processedObjects = 0;
        totalObjects = tempScheduleEntries.Count;
        ++processStage;

        // process all schedules
        foreach (string md5 in tempScheduleEntries.Keys)
        {
            ++processedObjects;
            ProcessMd5ScheduleEntry(md5);
        }
        logger.LogInformation("Processed {totalObjects} daily schedules for {Count} stations for average of {totalObjects:N1} days per station.",
            totalObjects, toProcess.Count, (double)totalObjects / toProcess.Count);
        logger.LogInformation("Exiting GetAllScheduleEntryMd5s(). SUCCESS.");
        epgCache.SaveCache();
        return true;
    }

    private async Task<bool> GetMd5ScheduleEntries(string[] dates, int start, Dictionary<string, string[]> tempScheduleEntries, CancellationToken cancellationToken)
    {

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        List<MxfService> toProcess = schedulesDirectData.Services.Values.ToList();
        // reject 0 requests
        if (toProcess.Count - start < 1)
        {
            return true;
        }


        // build request for station schedules

        ScheduleRequest[] requests = new ScheduleRequest[Math.Min(toProcess.Count - start, SchedulesDirect.MaxQueries / dates.Length)];

        for (int i = 0; i < requests.Length; ++i)
        {
            requests[i] = new ScheduleRequest()
            {
                StationId = toProcess[start + i].StationId,
                Date = dates
            };
        }


        // request schedule md5s from Schedules Direct
        Dictionary<string, Dictionary<string, ScheduleMd5Response>>? stationResponses = await GetScheduleMd5sAsync(requests, cancellationToken);
        if (stationResponses == null)
        {
            return false;
        }



        // build request of daily schedules not downloaded yet
        List<ScheduleRequest> newRequests = [];
        foreach (ScheduleRequest request in requests.Where(a => a is not null))
        {

            Dictionary<int, string> requestErrors = [];

            MxfService mxfService = schedulesDirectData.FindOrCreateService(request.StationId);
            IEnumerable<KeyValuePair<string, MxfService>> test = schedulesDirectData.Services.Where(arg => arg.Value.StationId.Equals(request.StationId));

            if (!schedulesDirectData.Services.ContainsKey(request.StationId))
            {
                int aaa = 1;
            }
            else
            {
                MxfService b = schedulesDirectData.Services[request.StationId];
            }

            if (stationResponses.TryGetValue(request.StationId, out Dictionary<string, ScheduleMd5Response>? stationResponse))
            {
                // if the station return is empty, go to next station
                if (stationResponse.Count == 0)
                {
                    string comment = $"Failed to parse the schedule Md5 return for stationId {mxfService.StationId} ({mxfService.CallSign}) on {dates[0]} and after.";
                    //if (CheckSuppressWarnings(mxfService.CallSign))
                    //{
                    //    logger.LogInformation(comment);
                    //}
                    //else
                    //{
                    logger.LogWarning(comment);
                    ++missingGuide;
                    //}
                    processedObjects += dates.Length;
                    continue;
                }

                // scan through all the dates returned for the station and request dates that are not cached
                List<string> newDateRequests = [];
                HashSet<string> dupeMd5s = [];
                foreach (string day in dates)
                {
                    if (stationResponse.TryGetValue(day, out ScheduleMd5Response? dayResponse) && (dayResponse.Code == 0) && !string.IsNullOrEmpty(dayResponse.Md5))
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
                            tempScheduleEntries.Add(dayResponse.Md5, [request.StationId, day]);
                        }
                        else
                        {

                            string previous = value[1];
                            string comment = $"Duplicate schedule Md5 return for stationId {mxfService.StationId} ({mxfService.CallSign}) on {day} with {previous}.";
                            logger.LogWarning(comment);
                            dupeMd5s.Add(dayResponse.Md5);
                        }
                    }
                    else if ((dayResponse != null) && (dayResponse.Code != 0) && !requestErrors.ContainsKey(dayResponse.Code))
                    {
                        requestErrors.Add(dayResponse.Code, dayResponse.Message);
                    }
                }

                // clear out dupe entries
                foreach (string dupe in dupeMd5s)
                {
                    string previous = tempScheduleEntries[dupe][1];
                    //string comment = $"Removing duplicate Md5 schedule entry for stationId {mxfService.StationId} ({mxfService.CallSign}) on {previous}.";
                    //logger.LogWarning(comment);
                    tempScheduleEntries.Remove(dupe);
                }

                // create the new request for the station
                if (newDateRequests.Count > 0)
                {
                    newRequests.Add(new ScheduleRequest()
                    {
                        StationId = request.StationId,
                        Date = [.. newDateRequests]
                    });
                }
            }
            else
            {
                // requested station was not in response
                logger.LogWarning($"Requested stationId {mxfService.StationId} ({mxfService.CallSign}) was not present in schedule Md5 response.");
                processedObjects += dates.Length;
                continue;
            }

            if (requestErrors.Count <= 0)
            {
                continue;
            }

            foreach (KeyValuePair<int, string> keyValuePair in requestErrors)
            {
                logger.LogWarning($"Requests for MD5 schedule entries of station {request.StationId} returned error code {keyValuePair.Key} , message: {keyValuePair.Value}");
            }
        }


        // download the remaining daily schedules to the cache directory
        if (newRequests.Count > 0)
        {
            // request daily schedules from Schedules Direct
            List<ScheduleResponse>? responses = await GetScheduleListingsAsync([.. newRequests]);
            if (responses == null)
            {
                return false;
            }

            // process the responses
            foreach (ScheduleResponse response in responses)
            {
                ++processedObjects;
                if (response?.Programs == null)
                {
                    continue;
                }
                ++downloadedSchedules;

                // serialize JSON directly to a file
                if (tempScheduleEntries.TryGetValue(response.Metadata.Md5, out string[]? serviceDate))
                {
                    try
                    {
                        JsonSerializerOptions jsonSerializerOptions = new()
                        {
                            // Add any desired JsonSerializerOptions here
                            // For example, to ignore null values during serialization:
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                            WriteIndented = false // Formatting.None equivalent
                        };

                        string jsonString = JsonSerializer.Serialize(response, jsonSerializerOptions);
                        epgCache.AddAsset(response.Metadata.Md5, jsonString);
                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("Failed to write station daily schedule file to cache file. station: {station} ; date: {date}. Exception: {ReportExceptionMessages}", serviceDate[0], serviceDate[1], FileUtil.ReportExceptionMessages(ex));
                    }
                }
                else
                {
                    try
                    {
                        KeyValuePair<string, string[]> compare = tempScheduleEntries
                            .Where(arg => arg.Value[0].Equals(response.StationId))
                            .Single(arg => arg.Value[1].Equals(response.Metadata.StartDate));
                        logger.LogWarning($"Md5 mismatch for station {compare.Value[0]} on {compare.Value[1]}. Expected: {compare.Key} - Downloaded: {response.Metadata.Md5}");
                    }
                    catch
                    {
                        logger.LogWarning($"Md5 mismatch for station {response.StationId} on {response.Metadata.StartDate}. Downloaded: {response.Metadata.Md5}");
                    }
                }
            }
        }

        return true;
    }

    private async Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] request)
    {
        DateTime dtStart = DateTime.Now;
        List<ScheduleResponse>? ret = await schedulesDirectAPI.GetApiResponse<List<ScheduleResponse>?>(APIMethod.POST, "schedules", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved {request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

    private void ProcessMd5ScheduleEntry(string md5)
    {
        // ensure cached file exists
        if (!epgCache.JsonFiles.ContainsKey(md5))
        {
            return;
        }

        // read the cached file
        ScheduleResponse? schedule = null;
        try
        {
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                // Add any desired JsonSerializerOptions here
                // For example, to ignore null values during deserialization:
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            string? asset = epgCache.GetAsset(md5);
            if (asset != null)
            {
                using StringReader reader = new(asset);
                string jsonString = reader.ReadToEnd();
                schedule = JsonSerializer.Deserialize<ScheduleResponse>(jsonString, jsonSerializerOptions);
            }

            if (schedule == null)
            {
                logger.LogError("Failed to read Md5Schedule entry in cache file.");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error occurred when trying to read Md5Schedule entry in cache file. Exception: {FileUtil.ReportExceptionMessages(ex)}");
            return;
        }


        // determine which service entry applies to
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        MxfService mxfService = schedulesDirectData.FindOrCreateService(schedule.StationId);

        // process each program schedule entry
        foreach (ScheduleProgram scheduleProgram in schedule.Programs)
        {

            // limit requests to airing programs now or in the future
            if (scheduleProgram.AirDateTime + TimeSpan.FromSeconds(scheduleProgram.Duration) < DateTime.UtcNow)
            {
                continue;
            }

            // prepopulate some of the program
            MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(scheduleProgram.ProgramId);

            if (mxfProgram.extras.Count == 0)
            {
                mxfProgram.UidOverride = $"{scheduleProgram.ProgramId[..10]}_{scheduleProgram.ProgramId[10..]}";
                mxfProgram.extras.Add("md5", scheduleProgram.Md5);
                if (scheduleProgram.Multipart?.PartNumber > 0)
                {
                    mxfProgram.extras.Add("multipart", $"{scheduleProgram.Multipart.PartNumber}/{scheduleProgram.Multipart.TotalParts}");
                }
                //if (config.OadOverride && scheduleProgram.New)
                //{
                //    mxfProgram.extras.Add("newAirDate", scheduleProgram.AirDateTime.ToLocalTime());
                //}
            }
            mxfProgram.IsSeasonFinale |= scheduleProgram.IsPremiereOrFinale.StringContains("Season Finale");
            mxfProgram.IsSeasonPremiere |= scheduleProgram.IsPremiereOrFinale.StringContains("Season Premiere");
            mxfProgram.IsSeriesFinale |= scheduleProgram.IsPremiereOrFinale.StringContains("Series Finale");
            mxfProgram.IsSeriesPremiere |= scheduleProgram.IsPremiereOrFinale.StringContains("Series Premiere");
            if (!mxfProgram.extras.ContainsKey("premiere"))
            {
                mxfProgram.extras.Add("premiere", false);
            }

            if (scheduleProgram.Premiere)
            {
                mxfProgram.extras.AddOrUpdate("premiere", true);
            }

            // grab any tvratings from desired countries
            Dictionary<string, string> scheduleTvRatings = [];
            if (scheduleProgram.Ratings != null)
            {
                //var origins = !string.IsNullOrEmpty(config.RatingsOrigin) ? config.RatingsOrigin.Split(',') : new[] { RegionInfo.CurrentRegion.ThreeLetterISORegionName };
                //if (Helper.TableContains(origins, "ALL"))
                //{
                foreach (ScheduleTvRating rating in scheduleProgram.Ratings)
                {
                    scheduleTvRatings.Add(rating.Body, rating.Code);
                }
                //}
                //else
                //{
                //    foreach (var origin in origins)
                //    {
                //        foreach (var rating in scheduleProgram.Ratings.Where(arg => arg.Country?.Equals(origin) ?? false))
                //        {
                //            scheduleTvRatings.Add(rating.Body, rating.Code);
                //        }
                //        if (scheduleTvRatings.Count > 0) break;
                //    }
                //}
            }

            // populate the schedule entry and create program entry as required
            mxfService.MxfScheduleEntries.ScheduleEntry.Add(new MxfScheduleEntry
            {
                AudioFormat = EncodeAudioFormat(scheduleProgram.AudioProperties),
                Duration = scheduleProgram.Duration,
                Is3D = SDHelpers.TableContains(scheduleProgram.VideoProperties, "3d"),
                IsBlackout = scheduleProgram.SubjectToBlackout,
                IsClassroom = scheduleProgram.CableInTheClassroom,
                IsCc = SDHelpers.TableContains(scheduleProgram.AudioProperties, "cc"),
                IsDelay = scheduleProgram.LiveTapeDelay.StringContains("delay"),
                IsDvs = SDHelpers.TableContains(scheduleProgram.AudioProperties, "dvs"),
                IsEnhanced = SDHelpers.TableContains(scheduleProgram.VideoProperties, "enhanced"),
                IsFinale = scheduleProgram.IsPremiereOrFinale.StringContains("finale"),
                IsHdtv = SDHelpers.TableContains(scheduleProgram.VideoProperties, "hd"),
                //IsHdtvSimulCast = null,
                IsInProgress = scheduleProgram.JoinedInProgress,
                IsLetterbox = SDHelpers.TableContains(scheduleProgram.VideoProperties, "letterbox"),
                IsLive = scheduleProgram.LiveTapeDelay.StringContains("live"),
                //IsLiveSports = null,
                IsPremiere = scheduleProgram.Premiere || scheduleProgram.IsPremiereOrFinale.StringContains("premiere"),
                IsRepeat = !scheduleProgram.New,
                IsSap = SDHelpers.TableContains(scheduleProgram.AudioProperties, "sap"),
                IsSubtitled = SDHelpers.TableContains(scheduleProgram.AudioProperties, "subtitled"),
                IsTape = scheduleProgram.LiveTapeDelay.StringContains("tape"),
                Part = scheduleProgram.Multipart?.PartNumber ?? 0,
                Parts = scheduleProgram.Multipart?.TotalParts ?? 0,
                mxfProgram = mxfProgram,
                StartTime = scheduleProgram.AirDateTime,
                //TvRating is determined in the class itself to combine with the program content ratings
                IsSigned = scheduleProgram.Signed
            });
            mxfService.MxfScheduleEntries.ScheduleEntry.Last().extras.Add("ratings", scheduleTvRatings);
        }
    }

    private int EncodeAudioFormat(string[] audioProperties)
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

    private async Task<Dictionary<string, Dictionary<string, ScheduleMd5Response>>?> GetScheduleMd5sAsync(ScheduleRequest[] request, CancellationToken cancellationToken)
    {
        DateTime dtStart = DateTime.Now;
        Dictionary<string, Dictionary<string, ScheduleMd5Response>>? ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, Dictionary<string, ScheduleMd5Response>>>(APIMethod.POST, "schedules/md5", request, cancellationToken);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved Md5s for {ret.Count}/{request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for Md5s of {request.Length} stations' daily schedules. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }


    private static string? SafeFilename(string md5)
    {
        return md5 == null ? null : Regex.Replace(md5, @"[^\w\.@-]", "-");
    }

    public void ResetCache()
    {
        cachedSchedules = 0;
        downloadedSchedules = 0;
        missingGuide = 0;
    }

    public void ClearCache()
    {
        epgCache.ResetCache();
    }
}