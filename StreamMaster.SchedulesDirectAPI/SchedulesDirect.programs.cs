using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Enums;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private async Task<bool> UpdateSDProgrammes(CancellationToken cancellationToken)
    {
       
        return true;
    }

    public  async Task<StationChannelMap?> GetStationChannelMap(string lineup)
    {
        StationChannelMap? ret =await schedulesDirectAPI.GetApiResponse<StationChannelMap>(APIMethod.GET, $"lineups/{lineup}");
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved the station mapping for lineup {lineup}. ({ret.Stations.Count} stations; {ret.Map.Count} channels)");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for retrieval of lineup {lineup}.");
        }

        return ret;
    }

    //private async Task<bool> UpdateSDProgrammes(CancellationToken cancellationToken)
    //{
    //    Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);

    //    int maxDays = setting.SDEPGDays;
    //    int maxRatings = setting.SDMaxRatings;
    //    bool useLineupInName = setting.SDUseLineupInName;

    //    List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);

    //    HashSet<string> stationsIds = setting.SDStationIds.Select(a => a.StationId).ToHashSet();
    //    Dictionary<string, Station> stationDictionary = stations.ToDictionary(s => s.StationId);

    //    List<ChannelLogoDto> channelLogos = memoryCache.ChannelLogos();
    //    int nextId = channelLogos.Any() ? channelLogos.Max(a => a.Id) + 1 : 0;

    //    // Process logos in parallel
    //    List<ChannelLogoDto> logoTasks =
    //    [
    //        .. setting.SDStationIds.AsParallel().WithCancellation(cancellationToken).SelectMany(SDStationId =>
    //                {
    //                    return stationDictionary.TryGetValue(SDStationId.StationId, out Station? station) && station.Lineup == SDStationId.Lineup
    //                        ? station.StationLogo?.Where(logo => !channelLogos.Any(a => a.Source == logo.URL))
    //                            .Select(logo => new ChannelLogoDto
    //                            {
    //                                Id = Interlocked.Increment(ref nextId),
    //                                Source = logo.URL,
    //                                Name = station.Name,
    //                                EPGId = "SD|" + SDStationId.StationId,
    //                                EPGFileId = 0
    //                            }) ?? Enumerable.Empty<ChannelLogoDto>()
    //                        : Enumerable.Empty<ChannelLogoDto>();
    //                }),
    //    ];

    //    if (logoTasks.Count != 0)
    //    {
    //        _logger.LogInformation("SD working on {num} logos", logoTasks.Count);
    //    }

    //    logoTasks.ForEach(memoryCache.Add);

    //    List<Schedule>? schedules = await GetSchedules([.. stationsIds], cancellationToken).ConfigureAwait(false);
    //    if (schedules?.Any() != true)
    //    {
    //        return false;
    //    }

    //    List<Programme> programmes = [];
    //    DateTime now = DateTime.Now;
    //    DateTime maxDate = now.AddDays(maxDays);

    //    if (schedules.Count != 0)
    //    {
    //        _logger.LogInformation("SD working on {num} schedules", schedules.Count);
    //    }

    //    int totalPrograms = schedules.SelectMany(a => a.Programs).Count(p => p.AirDateTime <= maxDate);

    //    _logger.LogInformation("SD working on {num} programs", totalPrograms);
    //    int counter = 0;

    //    foreach (Schedule sched in schedules)
    //    {
    //        if (!stationDictionary.TryGetValue(sched.StationID, out Station? station))
    //        {
    //            continue;
    //        }

    //        string channelNameSuffix = station.Name ?? sched.StationID;
    //        string displayName = useLineupInName ? $"{station.Lineup}-{channelNameSuffix}" : channelNameSuffix;
    //        string channelName = $"SD - {channelNameSuffix}";

    //        List<Program> relevantPrograms = sched.Programs.Where(p => p.AirDateTime <= maxDate).ToList();
    //        List<string> progIds = relevantPrograms.Select(p => p.ProgramID).Distinct().ToList();

    //        List<SDProgram> sdPrograms = await GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);
    //        await ProcessProgramsImages(sdPrograms, cancellationToken).ConfigureAwait(false);

    //        Dictionary<string, SDProgram> sdProgramsDict = sdPrograms.ToDictionary(p => p.ProgramID);

    //        foreach (Program? p in relevantPrograms)
    //        {
    //            counter++;
    //            if (counter % 100 == 0)
    //            {
    //                _logger.LogInformation("Processed {counter} programs out of {totalPrograms}", counter, totalPrograms);
    //            }

    //            if (!sdProgramsDict.TryGetValue(p.ProgramID, out SDProgram? sdProg))
    //            {
    //                continue;
    //            }

    //            DateTime startt = p.AirDateTime;
    //            DateTime endt = startt.AddSeconds(p.Duration);
    //            string lang = station.BroadcastLanguage.FirstOrDefault() ?? "en";

    //            Programme programme = new()
    //            {
    //                Start = startt.ToString("yyyyMMddHHmmss") + " +0000",
    //                Stop = endt.ToString("yyyyMMddHHmmss") + " +0000",
    //                Channel = $"SD|{sched.StationID}",
    //                ChannelName = channelName,
    //                //Name = channelNameSuffix,
    //                DisplayName = displayName,
    //                Title = SDHelpers.GetTitles(sdProg.Titles, lang),
    //                Subtitle = SDHelpers.GetSubTitles(sdProg, lang),
    //                Desc = SDHelpers.GetDescriptions(sdProg, lang),
    //                Credits = SDHelpers.GetCredits(sdProg),
    //                Category = SDHelpers.GetCategory(sdProg, lang),
    //                Language = lang,
    //                Episodenum = SDHelpers.GetEpisodeNums(sdProg),
    //                Icon = SDHelpers.GetIcons(sdProg, memoryCache),
    //                Rating = SDHelpers.GetRatings(sdProg, maxRatings),
    //                Video = SDHelpers.GetTvVideos(p),
    //                Audio = SDHelpers.GetTvAudios(p),
    //            };

    //            if (p.New != null)
    //            {
    //                programme.New = "";
    //            }
    //            else
    //            {
    //                programme.New = null;
    //                programme.Previouslyshown = SDHelpers.GetPreviouslyShown(sdProg);
    //            }

    //            if (!string.IsNullOrEmpty(p.LiveTapeDelay) && p.LiveTapeDelay.Equals("Live"))
    //            {
    //                programme.Live = "";
    //            }

    //            programmes.Add(programme);
    //        }
    //        _logger.LogInformation("Processed {counter} programs out of {totalPrograms}", counter, totalPrograms);
    //    }
    //    _logger.LogInformation("SD Finished");
    //    return memoryCache.SetSDProgreammesCache(programmes);
    //}

    //public async Task<List<Programme>> GetPrograms(List<string> programIds, CancellationToken cancellationToken)
    //{
    //    DateTime dtStart = DateTime.Now;
    //    List<string> distinctProgramIds = programIds.Distinct().ToList();
    //    List<SDProgram> results = [];
    //    List<string> programIdsToFetch = [];

    //    foreach (string? programId in distinctProgramIds)
    //    {
    //        SDProgram? cachedSchedule = await GetValidCachedDataAsync<SDProgram>("Program_" + programId, cancellationToken).ConfigureAwait(false);
    //        if (cachedSchedule != null)
    //        {
    //            results.Add(cachedSchedule);
    //        }
    //        else
    //        {
    //            programIdsToFetch.Add(programId);
    //        }
    //    }

    //    if (programIdsToFetch.Any())
    //    {
    //        List<SDProgram>? fetchedResults = await PostData<List<SDProgram>>("programs", programIdsToFetch, cancellationToken).ConfigureAwait(false);
    //        if (fetchedResults == null)
    //        {
    //            return [];
    //        }

    //        HashSet<string> processedProgramIds = [];

    //        foreach (SDProgram program in fetchedResults)
    //        {
    //            // Check if we've already processed this ProgramID
    //            if (!processedProgramIds.Contains(program.ProgramID))
    //            {
    //                // Write to cache if it's a new ProgramID
    //                await WriteToCacheAsync("Program_" + program.ProgramID, program, cancellationToken).ConfigureAwait(false);
    //                results.Add(program);
    //                // Mark this ProgramID as processed
    //                _ = processedProgramIds.Add(program.ProgramID);
    //            }
    //        }
    //    }

    //    return results;
    //}

    //public async Task<List<Schedule>> GetSchedules(List<string> stationIds, CancellationToken cancellationToken)
    //{
    //    List<string> distinctStationIds = stationIds.Distinct().ToList();
    //    List<Schedule> results = [];
    //    List<StationId> stationIdsToFetch = [];

    //    foreach (string? stationId in distinctStationIds)
    //    {
    //        List<Schedule>? cachedSchedule = await GetValidCachedDataAsync<List<Schedule>>("StationId_" + stationId, cancellationToken).ConfigureAwait(false);
    //        if (cachedSchedule != null)
    //        {
    //            results.AddRange(cachedSchedule);
    //        }
    //        else
    //        {
    //            stationIdsToFetch.Add(new StationId(stationId));
    //        }
    //    }

    //    if (stationIdsToFetch.Any())
    //    {
    //        List<Schedule>? fetchedResults = await PostData<List<Schedule>>("schedules", stationIdsToFetch, cancellationToken).ConfigureAwait(false);
    //        if (fetchedResults == null)
    //        {
    //            return [];
    //        }

    //        foreach (IGrouping<string, Schedule> group in fetchedResults.GroupBy(s => s.StationID))
    //        {
    //            string stationId = group.Key;
    //            List<Schedule> schedulesForStation = [.. group];
    //            await WriteToCacheAsync("StationId_" + stationId, schedulesForStation, cancellationToken).ConfigureAwait(false);

    //            //// Add the schedules to the results list'
    //            //foreach (Schedule t in schedulesForStation)
    //            //{
    //            //    //List<Schedule> schedulesForStations = Mapper.Map<List<Schedule>>(schedulesForStation);
    //            //    results.AddRange(schedulesForStation);
    //            //}

    //            //List<Schedule> schedulesForStations = Mapper.Map<List<Schedule>>(schedulesForStation);
    //            results.AddRange(schedulesForStation);
    //        }
    //    }

    //    return results;
    //}

    public async Task<List<Programme>?> GetProgramsAsync(string[] request)
    {
        DateTime dtStart = DateTime.Now;
        List<Programme>? ret = await schedulesDirectAPI.GetApiResponse<List<Programme>?>(APIMethod.POST, "programs", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved {ret.Count}/{request.Length} program descriptions. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} program descriptions. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

    public async Task<Dictionary<string, GenericDescription>?> GetGenericDescriptionsAsync(string[] request)
    {
        DateTime dtStart = DateTime.Now;
        Dictionary<string, GenericDescription>? ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, GenericDescription>?>(APIMethod.POST, "metadata/description/", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved {ret.Count}/{request.Length} generic program descriptions. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for {request.Length} generic program descriptions. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

}