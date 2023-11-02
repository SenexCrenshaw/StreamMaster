using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Models;
using StreamMasterDomain.Services;

using System.Net;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public class SchedulesDirect(ILogger<SchedulesDirect> logger, ISettingsService settingsService, IMemoryCache memoryCache) : ISchedulesDirect
{
    public static readonly int MAX_RETRIES = 2;
    private HttpClient _httpClient = null!;
    private SDToken _sdToken = null!;
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly ILogger _logger = logger;

    private SDToken SdToken
    {
        get
        {
            if (_sdToken == null)
            {
                Setting setting = settingsService.GetSettingsAsync().Result;
                _sdToken = new SDToken(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
            }
            return _sdToken;
        }
    }

    private HttpClient httpClient
    {
        get
        {
            if (_httpClient == null)
            {
                Setting setting = settingsService.GetSettingsAsync().Result;
                _httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
            }
            return _httpClient;
        }
    }

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


    public async Task Sync(List<StationIdLineUp> StationIdLineUps, CancellationToken cancellationToken)
    {
        SDStatus status = await GetStatus(cancellationToken);

        if (!await GetSystemReady(cancellationToken))
        {
            return;
        }
        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);
        DateTime now = DateTime.Now;

        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);
        List<Schedule>? schedules = await GetSchedules(StationIdLineUps.ConvertAll(a => a.StationId), cancellationToken).ConfigureAwait(false);
        List<string> progIds = schedules.SelectMany(a => a.Programs).Where(a => a.AirDateTime >= now.AddDays(-1) && a.AirDateTime <= now.AddDays(setting.SDEPGDays)).Select(a => a.ProgramID).Distinct().ToList();
        List<SDProgram> programs = await GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);
        memoryCache.SetSDProgreammesCache(programs);
    }

    public async Task<LineUpResult?> GetLineup(string lineUp, CancellationToken cancellationToken)
    {
        return await GetData<LineUpResult>($"lineups/{lineUp}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<LineUpPreview>> GetLineUpPreviews(CancellationToken cancellationToken)
    {
        List<LineUpPreview> res = new();
        List<Lineup>? lineups = await GetLineups(cancellationToken);

        if (lineups is null)
        {
            return res;
        }

        foreach (ILineup lineup in lineups)
        {
            List<LineUpPreview>? results = await GetData<List<LineUpPreview>>($"lineups/preview/{lineup.LineupString}", cancellationToken).ConfigureAwait(false);

            if (results == null)
            {
                continue;
            }

            for (int index = 0; index < results.Count; index++)
            {
                ILineUpPreview lineUpPreview = results[index];
                lineUpPreview.LineUp = lineup.LineupString;
                lineUpPreview.Id = index;
            }

            res.AddRange(results);

        }

        return res;
    }


    public async Task<bool> GetSystemReady(CancellationToken cancellationToken)
    {
        SDStatus status = await GetStatusInternal(cancellationToken);

        return status?.systemStatus[0].status?.ToLower() == "online";
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

    private async Task<SDStatus> GetStatusInternal(CancellationToken cancellationToken)
    {
        SDStatus? result = await GetData<SDStatus>("status", cancellationToken).ConfigureAwait(false);
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
                using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, cancellationToken).ConfigureAwait(false);

                if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    SdToken.LockOutToken();
                    return default;
                }

                if (responseCode == SDHttpResponseCode.TOKEN_EXPIRED || responseCode == SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetToken(cancellationToken).ConfigureAwait(false) == null)
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


    private async Task<T?> PostData<T>(string command, object toPost, CancellationToken cancellationToken, bool ignoreCache = false)
    {
        string jsonString = JsonSerializer.Serialize(toPost);

        StringContent content = new(jsonString, Encoding.UTF8, "application/json");
        string contentHash = SDHelpers.GenerateHashFromStringContent(content);
        string cacheKey = SDHelpers.GenerateCacheKey($"{command}_{contentHash}");
        string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);


        // Check if cache exists and is valid
        if (!ignoreCache && File.Exists(cachePath) && DateTime.UtcNow - File.GetLastWriteTimeUtc(cachePath) <= CacheDuration)
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
                using HttpResponseMessage response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, cancellationToken).ConfigureAwait(false);

                if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    SdToken.LockOutToken();
                    return default;
                }

                if (responseCode == SDHttpResponseCode.TOKEN_EXPIRED || responseCode == SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetToken(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }
                    continue;
                }

                if (result == null)
                {
                    return default;
                }
                if (!ignoreCache)
                {
                    SDCacheEntry<T> entry = new()
                    {
                        Timestamp = DateTime.UtcNow,
                        Command = command,
                        Content = "",
                        Data = result
                    };

                    string jsonResult = JsonSerializer.Serialize(entry);
                    await File.WriteAllTextAsync(cachePath, jsonResult, cancellationToken);
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
        SDStatus status = await GetStatus(cancellationToken);
        if (status.lineups == null || !status.lineups.Any())
        {
            return new();
        }

        return status.lineups.Where(a => !a.IsDeleted).ToList();

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
            List<SDProgram>? fetchedResults = await PostData<List<SDProgram>>("programs", programIdsToFetch, cancellationToken, true).ConfigureAwait(false);
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
                    int aaa = 1;
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

    public async Task<List<Schedule>> GetSchedules(List<string> stationIds, CancellationToken cancellationToken)
    {
        List<string> distinctStationIds = stationIds.Distinct().ToList();
        List<Schedule> results = new();
        List<StationId> stationIdsToFetch = new();

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
            List<Schedule>? fetchedResults = await PostData<List<Schedule>>("schedules", stationIdsToFetch, cancellationToken, true).ConfigureAwait(false);
            if (fetchedResults == null)
            {

                return new List<Schedule>();
            }

            foreach (IGrouping<string, Schedule> group in fetchedResults.GroupBy(s => s.StationID))
            {
                string stationId = group.Key;
                List<Schedule> schedulesForStation = group.ToList();
                await WriteToCacheAsync("StationId_" + stationId, schedulesForStation, cancellationToken).ConfigureAwait(false);

                // Add the schedules to the results list
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

            ret.Add(sp);
        }
        return ret;
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        List<Station> ret = new();

        List<Lineup> lineUps = await GetLineups(cancellationToken).ConfigureAwait(false);
        if (lineUps?.Any() != true)
        {
            return ret;
        }

        foreach (Lineup lineUp in lineUps)
        {

            ILineUpResult? res = await GetLineup(lineUp.LineupString, cancellationToken).ConfigureAwait(false);
            if (res == null)
            {
                continue;
            }


            foreach (Station station in res.Stations)
            {
                station.LineUp = lineUp.LineupString;
            }

            HashSet<string> existingIds = new(ret.Select(station => station.StationId));

            foreach (Station station in res.Stations)
            {
                station.LineUp = lineUp.LineupString;
                if (!existingIds.Contains(station.StationId))
                {
                    ret.Add(station);
                    existingIds.Add(station.StationId);
                }
            }
        }

        return ret;
    }

    public async Task<string> GetEpg(CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);
        List<StationIdLineUp> stationIdLineUps = setting.SDStationIds;
        List<string> stationsIds = stationIdLineUps.ConvertAll(a => a.StationId).Distinct().ToList();

        await Sync(stationIdLineUps, cancellationToken);

        List<Schedule>? schedules = await GetSchedules(stationsIds, cancellationToken);

        if (schedules?.Any() != true)
        {
            Console.WriteLine("No schedules");
            return FileUtil.SerializeEpgData(new Tv());
        }

        List<TvChannel> retChannels = new();
        List<Programme> retProgrammes = new();

        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);

        foreach (string? stationId in stationsIds)
        {
            List<string> names = stations.Where(a => a.StationId == stationId).Select(a => a.Name).Distinct().ToList();
            List<List<StationLogo>> logos = stations.Where(a => a.StationId == stationId).Select(a => a.StationLogo).Distinct().ToList();
            ILogo? logo = stations.Where(a => a.StationId == stationId).Select(a => a.Logo).FirstOrDefault();
            IStation station = stations.First(a => a.StationId == stationId);

            TvChannel channel = new()
            {
                Id = stationId,
                Displayname = names,

            };

            if (station.Logo != null)
            {
                channel.Icon = new TvIcon
                {
                    Src = station.Logo.URL
                };
            }

            retChannels.Add(channel);
        }

        List<string> progIds = schedules.SelectMany(a => a.Programs).Select(a => a.ProgramID).Distinct().ToList();
        List<SDProgram> programs = await GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);

        foreach (SDProgram sdProg in programs)
        {
            foreach (Schedule sched in schedules.Where(a => a.Programs.Any(a => a.ProgramID == sdProg.ProgramID)).ToList())
            {
                IStation station = stations.First(a => a.StationId == sched.StationID);

                foreach (Program p in sched.Programs)
                {
                    DateTime startt = p.AirDateTime;
                    DateTime endt = startt.AddSeconds(p.Duration);
                    string lang = "en";
                    if (station.BroadcastLanguage.Any())
                    {
                        lang = station.BroadcastLanguage[0];
                    }

                    Programme programme = new()
                    {
                        Start = startt.ToString("yyyyMMddHHmmss") + " +0000",
                        Stop = endt.ToString("yyyyMMddHHmmss") + " +0000",
                        Channel = sched.StationID,

                        Title = SDHelpers.GetTitles(sdProg.Titles, lang),
                        Subtitle = SDHelpers.GetSubTitles(sdProg, lang),
                        Desc = SDHelpers.GetDescriptions(sdProg, lang),
                        Credits = SDHelpers.GetCredits(sdProg, lang),
                        Category = SDHelpers.GetCategory(sdProg, lang),
                        Language = lang,
                        Episodenum = SDHelpers.GetEpisodeNums(sdProg, lang),
                        Icon = SDHelpers.GetIcons(p, sdProg, sched, lang),
                        Rating = SDHelpers.GetRatings(sdProg, lang, setting.SDMaxRatings),
                        Video = SDHelpers.GetTvVideos(p),
                        Audio = SDHelpers.GetTvAudios(p),

                    };

                    if (p.New is not null)
                    {
                        programme.New = ((bool)p.New).ToString();
                    }

                    if (!string.IsNullOrEmpty(p.LiveTapeDelay) && p.LiveTapeDelay.Equals("Live"))
                    {
                        programme.Live = "";
                    }

                    retProgrammes.Add(programme);
                }
            }
        }

        Tv tv = new()
        {
            Channel = retChannels.OrderBy(a => int.Parse(a!.Id)).ToList(),
            Programme = retProgrammes.OrderBy(a => int.Parse(a.Channel)).ThenBy(a => a.StartDateTime).ToList()
        };
        return FileUtil.SerializeEpgData(tv);
    }

}
