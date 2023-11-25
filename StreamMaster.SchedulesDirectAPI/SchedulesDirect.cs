using AutoMapper;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Commands;
using StreamMaster.SchedulesDirectAPI.Domain.EPG;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;
using StreamMasterDomain.Services;

using System.Net;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public class SchedulesDirect(ILogger<SchedulesDirect> logger, ISettingsService settingsService, ISDToken SdToken, IMemoryCache memoryCache) : ISchedulesDirect
{
    public static readonly int MAX_RETRIES = 2;
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly ILogger _logger = logger;

    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        return await GetData<Countries>("available/countries", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        return await GetData<List<Headend>>($"headends?country={country}&postalcode={postalCode}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> GetImageUrl(string source, string fileName, CancellationToken cancellationToken)
    {
        string? url = await SdToken.GetAPIUrl($"image/{source}", cancellationToken);
        if (url == null)
        {
            _logger.LogWarning("Image URL for source {Source} not found.", source);
            return false;
        }

        (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(url, fileName, cancellationToken).ConfigureAwait(false);

        if (!success && ex != null)
        {
            _logger.LogError(ex, "Failed to download image from {Url} to {FileName}.", url, fileName);
        }

        return success;
    }

    public async Task<bool> SDSync(List<StationIdLineup> StationIdLineups, CancellationToken cancellationToken)
    {
        //SDStatus status = await GetStatus(cancellationToken);

        if (!await GetSystemReady(cancellationToken))
        {
            _logger.LogWarning("SD Not Ready");
            return false;
        }

        //DateTime now = DateTime.Now;

        //List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);
        //List<Schedule>? schedules = await GetSchedules(StationIdLineups.ConvertAll(a => a.StationId), cancellationToken).ConfigureAwait(false);
        //List<string> progIds = schedules.SelectMany(a => a.Programs).Where(a => a.AirDateTime >= now.AddDays(-1) && a.AirDateTime <= now.AddDays(setting.SDEPGDays)).Select(a => a.ProgramID).Distinct().ToList();
        //List<SDProgram> programs = await GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);

        return await UpdateSDProgrammes(cancellationToken).ConfigureAwait(false);
    }

    private async Task<bool> UpdateSDProgrammes(CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);

        int maxDays = setting.SDEPGDays;
        int maxRatings = setting.SDMaxRatings;
        bool useLineupInName = setting.SDUseLineupInName;

        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);

        HashSet<string> stationsIds = setting.SDStationIds.Select(a => a.StationId).ToHashSet();
        Dictionary<string, Station> stationDictionary = stations.ToDictionary(s => s.StationId);

        List<ChannelLogoDto> channelLogos = memoryCache.ChannelLogos();
        int nextId = channelLogos.Any() ? channelLogos.Max(a => a.Id) + 1 : 0;

        // Process logos in parallel
        List<ChannelLogoDto> logoTasks = setting.SDStationIds.AsParallel().WithCancellation(cancellationToken).SelectMany(SDStationId =>
        {
            if (stationDictionary.TryGetValue(SDStationId.StationId, out Station? station) && station.Lineup == SDStationId.Lineup)
            {
                return station.StationLogo?.Where(logo => !channelLogos.Any(a => a.LogoUrl == logo.URL))
                    .Select(logo => new ChannelLogoDto
                    {
                        Id = Interlocked.Increment(ref nextId),
                        LogoUrl = logo.URL,
                        EPGId = "SD|" + SDStationId.StationId,
                        EPGFileId = 0
                    }) ?? Enumerable.Empty<ChannelLogoDto>();
            }
            return Enumerable.Empty<ChannelLogoDto>();
        }).ToList();

        _logger.LogInformation("SD working on {num} logos", logoTasks.Count);

        logoTasks.ForEach(logo => memoryCache.Add(logo));

        List<Schedule>? schedules = await GetSchedules(stationsIds.ToList(), cancellationToken).ConfigureAwait(false);
        if (schedules == null || !schedules.Any())
        {
            return false;
        }

        List<Programme> programmes = new();
        DateTime now = DateTime.Now;
        DateTime maxDate = now.AddDays(maxDays);

        _logger.LogInformation("SD working on {num} schedules", schedules.Count);
        foreach (Schedule sched in schedules)
        {
            if (!stationDictionary.TryGetValue(sched.StationID, out Station? station))
            {
                continue;
            }

            string channelNameSuffix = station.Name ?? sched.StationID;
            string displayName = useLineupInName ? $"{station.Lineup}-{channelNameSuffix}" : channelNameSuffix;
            string channelName = $"SD - {channelNameSuffix}";

            List<Program> relevantPrograms = sched.Programs.Where(p => p.AirDateTime <= maxDate).ToList();
            List<string> progIds = relevantPrograms.Select(p => p.ProgramID).Distinct().ToList();

            List<SDProgram> sdPrograms = await GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);
            Dictionary<string, SDProgram> sdProgramsDict = sdPrograms.ToDictionary(p => p.ProgramID);

            int counter = 0;
            _logger.LogInformation("SD working on {num} programs", relevantPrograms.Count);
            foreach (Program? p in relevantPrograms)
            {
                counter++;
                if (counter % 100 == 0)
                {
                    _logger.LogInformation("Processed {counter} programs out of {totalPrograms}", counter, relevantPrograms.Count);
                }

                if (!sdProgramsDict.TryGetValue(p.ProgramID, out SDProgram? sdProg))
                {
                    continue;
                }



                DateTime startt = p.AirDateTime;
                DateTime endt = startt.AddSeconds(p.Duration);
                string lang = station.BroadcastLanguage.FirstOrDefault() ?? "en";

                Programme programme = new()
                {
                    Start = startt.ToString("yyyyMMddHHmmss") + " +0000",
                    Stop = endt.ToString("yyyyMMddHHmmss") + " +0000",
                    Channel = $"SD|{sched.StationID}",
                    ChannelName = channelName,
                    Name = channelNameSuffix,
                    DisplayName = displayName,
                    Title = SDHelpers.GetTitles(sdProg.Titles, lang),
                    Subtitle = SDHelpers.GetSubTitles(sdProg, lang),
                    Desc = SDHelpers.GetDescriptions(sdProg, lang),
                    Credits = SDHelpers.GetCredits(sdProg, lang),
                    Category = SDHelpers.GetCategory(sdProg, lang),
                    Language = lang,
                    Episodenum = SDHelpers.GetEpisodeNums(sdProg, lang),
                    Icon = SDHelpers.GetIcons(p, sdProg, sched, lang),
                    Rating = SDHelpers.GetRatings(sdProg, lang, maxRatings),
                    Video = SDHelpers.GetTvVideos(p),
                    Audio = SDHelpers.GetTvAudios(p),
                };

                programmes.Add(programme);
            }
        }

        return memoryCache.SetSDProgreammesCache(programmes);
    }

    public async Task<LineupResult?> GetLineup(string lineup, CancellationToken cancellationToken)
    {
        return await GetData<LineupResult>($"lineups/{lineup}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<LineupPreview>> GetLineupPreviews(CancellationToken cancellationToken)
    {
        List<LineupPreview> res = new();
        List<Lineup>? lineups = await GetLineups(cancellationToken);

        if (lineups is null)
        {
            return res;
        }

        foreach (ILineup lineup in lineups)
        {
            List<LineupPreview>? results = await GetData<List<LineupPreview>>($"lineups/preview/{lineup.LineupString}", cancellationToken).ConfigureAwait(false);

            if (results == null)
            {
                continue;
            }

            for (int index = 0; index < results.Count; index++)
            {
                ILineupPreview lineupPreview = results[index];
                lineupPreview.Lineup = lineup.LineupString;
                lineupPreview.Id = index;
                lineupPreview.Affiliate ??= "";
            }

            res.AddRange(results);
        }

        return res;
    }

    public async Task<bool> GetSystemReady(CancellationToken cancellationToken)
    {
        SDStatus status = await GetStatusInternal(cancellationToken);

        try
        {
            if (status.systemStatus.Any())
            {
                return status.systemStatus[0].status?.ToLower() == "online";
            }
            return false;
        }
        catch (Exception ex)
        {

        }
        return false;

    }

    public async Task<SDStatus> GetStatus(CancellationToken cancellationToken)
    {
        SDStatus status = await GetStatusInternal(cancellationToken);
        if (status == null)
        {
            return SDHelpers.GetSDStatusOffline();
        }
        return status;
    }

    public void ResetCache(string command)
    {
        string cacheKey = SDHelpers.GenerateCacheKey(command);
        string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);

        if (File.Exists(cachePath))
        {
            File.Delete(cachePath);
        }
    }

    private async Task<SDStatus> GetStatusInternal(CancellationToken cancellationToken)
    {
        SDStatus? result = await GetData<SDStatus>(SDCommands.Status, cancellationToken).ConfigureAwait(false);
        return result ?? SDHelpers.GetSDStatusOffline();
    }

    private async Task<T?> GetData<T>(string command, CancellationToken cancellationToken)
    {
        string cacheKey = SDHelpers.GenerateCacheKey(command);
        string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);

        // Check if cache exists and is valid
        if (File.Exists(cachePath) && DateTime.UtcNow - File.GetLastWriteTimeUtc(cachePath) <= CacheDuration)
        {
            string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken);
            SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);

            if (cacheEntry != null && DateTime.UtcNow - cacheEntry.Timestamp <= CacheDuration)
            {
                return cacheEntry.Data;
            }
        }

        int retry = 0;
        try
        {
            while (retry <= MAX_RETRIES)
            {
                string? url = await SdToken.GetAPIUrl(command, cancellationToken);

                Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

                HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
                using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, cancellationToken).ConfigureAwait(false);

                if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
                    return default;
                }

                if (responseCode == SDHttpResponseCode.TOKEN_EXPIRED || responseCode == SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }
                    ++retry;
                    continue;
                }

                if (result == null)
                {
                    return default;
                }

                SDCacheEntry<T> entry = new()
                {
                    Timestamp = DateTime.UtcNow,
                    Command = command,
                    Content = "",
                    Data = result
                };

                string jsonResult = JsonSerializer.Serialize(entry);
                await File.WriteAllTextAsync(cachePath, jsonResult, cancellationToken);
                return result;
            }
        }
        catch (Exception)
        {
            return default;
        }
        return default;
    }

    private async Task<T?> PostData<T>(string command, object toPost, CancellationToken cancellationToken)
    {
        string jsonString = JsonSerializer.Serialize(toPost);

        StringContent content = new(jsonString, Encoding.UTF8, "application/json");

        int retry = 0;
        try
        {
            while (retry <= MAX_RETRIES)
            {
                string? url = await SdToken.GetAPIUrl(command, cancellationToken);
                Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

                HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
                using HttpResponseMessage response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, cancellationToken).ConfigureAwait(false);

                if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
                    return default;
                }

                if (responseCode == SDHttpResponseCode.TOKEN_EXPIRED || responseCode == SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }
                    continue;
                }

                if (result == null)
                {
                    return default;
                }

                return result;
            }
        }
        catch (Exception ex)
        {
            return default;
        }
        return default;
    }

    private async Task<PutResponse?> DeleteData(string command, CancellationToken cancellationToken)
    {
        int retry = 0;
        try
        {
            while (retry <= MAX_RETRIES)
            {
                string? url = await SdToken.GetAPIUrl(command, cancellationToken);

                Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

                HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
                using HttpResponseMessage response = await httpClient.DeleteAsync(url).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, PutResponse? result) = await SDHandler.ProcessResponse<PutResponse?>(response, cancellationToken).ConfigureAwait(false);

                if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
                    return default;
                }

                if (responseCode == SDHttpResponseCode.TOKEN_EXPIRED || responseCode == SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }
                    ++retry;
                    continue;
                }

                if (result == null)
                {
                    return default;
                }

                return result;
            }
        }
        catch (Exception)
        {
            return default;
        }
        return default;
    }

    private async Task<PutResponse?> PutData(string command, CancellationToken cancellationToken)
    {
        int retry = 0;
        try
        {
            while (retry <= MAX_RETRIES)
            {
                string? url = await SdToken.GetAPIUrl(command, cancellationToken);

                Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

                HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
                using HttpResponseMessage response = await httpClient.PutAsync(url, null).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, PutResponse? result) = await SDHandler.ProcessResponse<PutResponse?>(response, cancellationToken).ConfigureAwait(false);

                if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
                    return default;
                }

                if (responseCode == SDHttpResponseCode.TOKEN_EXPIRED || responseCode == SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }
                    ++retry;
                    continue;
                }

                if (result == null)
                {
                    return default;
                }

                return result;
            }
        }
        catch (Exception)
        {
            return default;
        }
        return default;
    }

    public async Task<List<Lineup>> GetLineups(CancellationToken cancellationToken)
    {
        LineupsResult? res = await GetData<LineupsResult>(SDCommands.LineUps, cancellationToken).ConfigureAwait(false);
        if (res == null)
        {
            return new();
        }
        List<Lineup> lineups = res.Lineups.Where(a => !a.IsDeleted).ToList();
        return lineups;
    }

    public async Task<List<SDProgram>> GetSDPrograms(List<string> programIds, CancellationToken cancellationToken)
    {
        List<string> distinctProgramIds = programIds.Distinct().ToList();
        List<SDProgram> results = new();
        List<string> programIdsToFetch = new();

        foreach (string? programId in distinctProgramIds)
        {
            SDProgram? cachedSchedule = await GetValidCachedDataAsync<SDProgram>("Program_" + programId, cancellationToken).ConfigureAwait(false);
            if (cachedSchedule != null)
            {
                results.Add(cachedSchedule);
            }
            else
            {
                programIdsToFetch.Add(programId);
            }
        }

        if (programIdsToFetch.Any())
        {
            List<SDProgram>? fetchedResults = await PostData<List<SDProgram>>("programs", programIdsToFetch, cancellationToken).ConfigureAwait(false);
            if (fetchedResults == null)
            {
                return new List<SDProgram>();
            }

            HashSet<string> processedProgramIds = new();

            foreach (SDProgram program in fetchedResults)
            {
                // Check if we've already processed this ProgramID
                if (!processedProgramIds.Contains(program.ProgramID))
                {
                    // Write to cache if it's a new ProgramID
                    await WriteToCacheAsync("Program_" + program.ProgramID, program, cancellationToken).ConfigureAwait(false);
                    results.Add(program);
                    // Mark this ProgramID as processed
                    processedProgramIds.Add(program.ProgramID);
                }
                else
                {
                }
            }
        }

        return results;
    }

    private async Task WriteToCacheAsync<T>(string name, T data, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);
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
            _cacheSemaphore.Release();
        }
    }

    private async Task<T?> GetValidCachedDataAsync<T>(string name, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);
            if (!File.Exists(cachePath))
            {
                return default;
            }

            string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken).ConfigureAwait(false);
            SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);

            if (cacheEntry != null && DateTime.Now - cacheEntry.Timestamp <= CacheDuration)
            {
                return cacheEntry.Data;
            }

            return default;
        }
        finally
        {
            _cacheSemaphore.Release();
        }
    }

    public async Task<bool> AddLineup(string lineup, CancellationToken cancellationToken)
    {
        PutResponse? fetchedResults = await PutData($"lineups/{lineup}", cancellationToken).ConfigureAwait(false);
        if (fetchedResults == null)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken)
    {
        PutResponse? fetchedResults = await DeleteData($"lineups/{lineup}", cancellationToken).ConfigureAwait(false);
        if (fetchedResults == null)
        {
            return false;
        }

        return true;
    }

    public async Task<List<Schedule>> GetSchedules(List<string> stationIds, CancellationToken cancellationToken)
    {
        List<string> distinctStationIds = stationIds.Distinct().ToList();
        List<Schedule> results = [];
        List<StationId> stationIdsToFetch = [];

        foreach (string? stationId in distinctStationIds)
        {
            List<Schedule>? cachedSchedule = await GetValidCachedDataAsync<List<Schedule>>("StationId_" + stationId, cancellationToken).ConfigureAwait(false);
            if (cachedSchedule != null)
            {
                results.AddRange(cachedSchedule);
            }
            else
            {
                stationIdsToFetch.Add(new StationId(stationId));
            }
        }

        if (stationIdsToFetch.Any())
        {
            List<Schedule>? fetchedResults = await PostData<List<Schedule>>("schedules", stationIdsToFetch, cancellationToken).ConfigureAwait(false);
            if (fetchedResults == null)
            {
                return [];
            }

            foreach (IGrouping<string, Schedule> group in fetchedResults.GroupBy(s => s.StationID))
            {
                string stationId = group.Key;
                List<Schedule> schedulesForStation = [.. group];
                await WriteToCacheAsync("StationId_" + stationId, schedulesForStation, cancellationToken).ConfigureAwait(false);

                //// Add the schedules to the results list'
                //foreach (Schedule t in schedulesForStation)
                //{
                //    //List<Schedule> schedulesForStations = Mapper.Map<List<Schedule>>(schedulesForStation);
                //    results.AddRange(schedulesForStation);
                //}

                //List<Schedule> schedulesForStations = Mapper.Map<List<Schedule>>(schedulesForStation);
                results.AddRange(schedulesForStation);
            }
        }

        return results;
    }

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        List<Station>? stations = await GetStations(cancellationToken).ConfigureAwait(false);
        if (stations is null)
        {
            return new();
        }
        List<StationPreview> ret = new();
        for (int index = 0; index < stations.Count; index++)
        {
            IStation station = stations[index];
            StationPreview sp = new(station);
            sp.Affiliate ??= "";
            ret.Add(sp);
        }
        return ret;
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        List<Station> ret = new();

        List<Domain.Models.Lineup> lineups = await GetLineups(cancellationToken).ConfigureAwait(false);
        if (lineups?.Any() != true)
        {
            return ret;
        }

        foreach (Lineup lineup in lineups)
        {
            ILineupResult? res = await GetLineup(lineup.LineupString, cancellationToken).ConfigureAwait(false);
            if (res == null)
            {
                continue;
            }

            foreach (Station station in res.Stations)
            {
                station.Lineup = lineup.LineupString;
            }

            HashSet<string> existingIds = new(ret.Select(station => station.StationId));

            foreach (Station station in res.Stations)
            {
                station.Lineup = lineup.LineupString;
                if (!existingIds.Contains(station.StationId))
                {
                    ret.Add(station);
                    existingIds.Add(station.StationId);
                }
            }
        }

        return ret;
    }
}