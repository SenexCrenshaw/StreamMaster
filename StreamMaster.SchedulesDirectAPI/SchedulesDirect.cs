using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;

using StreamMasterDomain.Common;
using StreamMasterDomain.Models;
using StreamMasterDomain.Services;

using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public class SchedulesDirect(ILogger<SchedulesDirect> logger, ISettingsService settingsService) : ISchedulesDirect
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
                _httpClient = CreateHttpClient(setting.ClientUserAgent);
            }
            return _httpClient;
        }
    }


    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        Countries? result = await GetData<Countries>("available/countries", cancellationToken).ConfigureAwait(false);

        return result;
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        List<Headend>? result = await GetData<List<Headend>>($"headends?country={country}&postalcode={postalCode}", cancellationToken).ConfigureAwait(false);

        return result ?? null;
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


    public async Task<List<SDProgram>> Sync(List<StationIdLineUp> StationIdLineUps, CancellationToken cancellationToken)
    {
        if (!await GetSystemReady(cancellationToken))
        {
            return new();
        }
        SDStatus status = await GetStatus(cancellationToken);

        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);
        List<Schedule>? schedules = await GetSchedules(StationIdLineUps.ConvertAll(a => a.StationId), cancellationToken).ConfigureAwait(false);
        List<string> progIds = schedules.SelectMany(a => a.Programs).Where(a => a.AirDateTime <= DateTime.Now.AddDays(1)).Select(a => a.ProgramID).Distinct().ToList();
        List<SDProgram> programs = await GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);
        return programs;
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

    private string GenerateCacheKey(string command)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        StringBuilder sanitized = new(command.Length);
        foreach (char c in command)
        {
            if (!invalidChars.Contains(c))
            {
                sanitized.Append(c);
            }
            else
            {
                sanitized.Append('_');  // replace invalid chars with underscore or another desired character
            }
        }
        sanitized.Append(".json");
        return sanitized.ToString();
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
            return GetSDStatusOffline();
        }
        return status;
    }

    private static SDStatus GetSDStatusOffline()
    {
        SDStatus ret = new();
        ret.systemStatus.Add(new SDSystemStatus { status = "Offline" });
        return ret;
    }

    private async Task<SDStatus> GetStatusInternal(CancellationToken cancellationToken)
    {
        SDStatus? result = await GetData<SDStatus>("status", cancellationToken).ConfigureAwait(false);
        return result ?? GetSDStatusOffline();
    }

    private async Task<T?> GetData<T>(string command, CancellationToken cancellationToken)
    {
        string cacheKey = GenerateCacheKey(command);
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
    private string GenerateHashFromStringContent(StringContent content)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] contentBytes = Encoding.UTF8.GetBytes(content.ReadAsStringAsync().Result); // Extract string from StringContent and convert to bytes
        byte[] hashBytes = sha256.ComputeHash(contentBytes); // Compute SHA-256 hash
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Convert byte array to hex string
    }

    private async Task<T?> PostData<T>(string command, object toPost, CancellationToken cancellationToken, bool ignoreCache = false)
    {
        string jsonString = JsonSerializer.Serialize(toPost);

        StringContent content = new(jsonString, Encoding.UTF8, "application/json");
        string contentHash = GenerateHashFromStringContent(content);
        string cacheKey = GenerateCacheKey($"{command}_{contentHash}");
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
            string cacheKey = GenerateCacheKey(name);
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
            string cacheKey = GenerateCacheKey(name);
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

    private static HttpClient CreateHttpClient(string clientUserAgent)
    {
        HttpClient client = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        });
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(clientUserAgent);

        return client;
    }

    public static List<TvTitle> GetTitles(List<Title> Titles, string lang)
    {

        List<TvTitle> ret = Titles.ConvertAll(a => new TvTitle
        {
            Lang = lang,
            Text = a.Title120
        });
        return ret;
    }

    public static TvSubtitle GetSubTitles(ISDProgram sdProgram, string lang)
    {
        if (!string.IsNullOrEmpty(sdProgram.EpisodeTitle150))
        {
            return new TvSubtitle
            {
                Lang = lang,
                Text = sdProgram.EpisodeTitle150
            };
        }

        string description = "";
        if (sdProgram.Descriptions?.Description100 is not null)
        {
            IDescription100? test = sdProgram.Descriptions.Description100.FirstOrDefault(a => a.DescriptionLanguage == lang && a.Description != "");
            if (test == null)
            {
                if (!string.IsNullOrEmpty(sdProgram.Descriptions.Description100[0].Description))
                {
                    description = sdProgram.Descriptions.Description100[0].Description;
                }
            }
            else
            {
                description = test.Description;
            }
        }

        return new TvSubtitle
        {
            Lang = lang,
            Text = description
        };
    }

    public static TvCredits GetCredits(ISDProgram sdProgram, string lang)
    {
        TvCredits ret = new();
        if (sdProgram.Crew == null)
        {
            return ret;

        }
        foreach (Crew crew in sdProgram.Crew)
        {
            switch (crew.Role)
            {
                case "Actor":
                    ret.Actor ??= new();
                    ret.Actor.Add(new TvActor
                    {
                        Text = crew.Name,
                        Role = crew.Role
                    });
                    break;
                case "Director":
                    ret.Director ??= new();
                    ret.Director.Add(crew.Name);
                    break;
                case "Producer":
                    ret.Producer ??= new();
                    ret.Producer.Add(crew.Name);
                    break;
                case "Presenter":
                    ret.Presenter ??= new();
                    ret.Presenter.Add(crew.Name);
                    break;
                case "Writer":
                    ret.Writer ??= new();
                    ret.Writer.Add(crew.Name);
                    break;
            }
        }

        if (sdProgram.Cast is not null)
        {
            foreach (ICast? cast in sdProgram.Cast.OrderBy(a => a.BillingOrder))
            {
                ret.Actor ??= new();
                ret.Actor.Add(new TvActor
                {
                    Text = cast.Name,
                    Role = cast.CharacterName
                });
                break;
            }
        }

        return ret;
    }

    public static TvDesc GetDescriptions(ISDProgram sdProgram, string lang)
    {

        string description = "";
        if (sdProgram.Descriptions is not null)
        {
            if (sdProgram.Descriptions.Description1000 is not null && sdProgram.Descriptions.Description1000.Any())
            {
                IDescription1000? test = sdProgram.Descriptions.Description1000.FirstOrDefault(a => a.DescriptionLanguage == lang && a.Description != "");
                if (test == null)
                {
                    if (!string.IsNullOrEmpty(sdProgram.Descriptions.Description1000[0].Description))
                    {
                        description = sdProgram.Descriptions.Description1000[0].Description;
                    }
                }
                else
                {
                    description = test.Description;
                }
            }

        }

        return new TvDesc
        {
            Lang = lang,
            Text = description
        };
    }

    public static List<TvCategory> GetCategory(ISDProgram sdProgram, string lang)
    {
        List<TvCategory> ret = new();

        if (sdProgram.Genres is not null)
        {
            foreach (string genre in sdProgram.Genres)
            {
                ret.Add(new TvCategory
                {
                    Lang = lang,
                    Text = genre
                });
            }
        }

        return ret;
    }

    public static List<TvEpisodenum> GetEpisodeNums(ISDProgram sdProgram, string lang)
    {
        List<TvEpisodenum> ret = new();
        int season = 0;
        int episode = 0;

        if (sdProgram.Metadata is not null && sdProgram.Metadata.Any())
        {
            foreach (ProgramMetadata m in sdProgram.Metadata.Where(a => a.Gracenote != null))
            {
                season = m.Gracenote.Season;
                episode = m.Gracenote.Episode;
                ret.Add(new TvEpisodenum
                {
                    System = "xmltv_ns",
                    Text = $"{season:00}.{episode:00}"
                });
            }
        }

        if (season != 0 && episode != 0)
        {
            ret.Add(new TvEpisodenum
            {
                System = "onscreen",
                Text = $"S{season} E{episode}"
            });
        }

        if (ret.Count == 0)
        {
            string prefix = sdProgram.ProgramID[..2];
            string newValue;

            switch (prefix)
            {
                case "EP":
                    newValue = sdProgram.ProgramID[..10] + "." + sdProgram.ProgramID[10..];
                    break;

                case "SH":
                case "MV":
                    newValue = sdProgram.ProgramID[..10] + ".0000";
                    break;

                default:
                    newValue = sdProgram.ProgramID;
                    break;
            }
            ret.Add(new TvEpisodenum
            {
                System = "dd_progid",
                Text = newValue
            });
        }

        if (sdProgram.OriginalAirDate != null)
        {
            ret.Add(new TvEpisodenum
            {
                System = "original-air-date",
                Text = sdProgram.OriginalAirDate
            });
        }

        return ret;

    }

    public static List<TvIcon> GetIcons(IProgram program, ISDProgram sdProgram, ISchedule sched, string lang)
    {
        List<TvIcon> ret = new();
        List<string> aspects = new() { "2x3", "4x3", "3x4", "16x9" };

        if (sdProgram.Metadata is not null && sdProgram.Metadata.Any())
        {

        }

        return ret;

    }

    public static List<TvRating> GetRatings(ISDProgram sdProgram, string countryCode, int maxRatings)
    {
        List<TvRating> ratings = new();


        if (sdProgram?.ContentRating == null)
        {

            return ratings;
        }

        maxRatings = maxRatings > 0 ? Math.Min(maxRatings, sdProgram.ContentRating.Count) : sdProgram.ContentRating.Count;

        foreach (ContentRating? cr in sdProgram.ContentRating.Take(maxRatings))
        {
            ratings.Add(new TvRating
            {
                System = cr.Body,
                Value = cr.Code
            });
        }

        return ratings;
    }


    public static TvVideo GetTvVideos(Program sdProgram)
    {
        TvVideo ret = new();

        if (sdProgram.VideoProperties?.Any() == true)
        {
            ret.Quality = sdProgram.VideoProperties.ToList();
        }

        return ret;

    }

    public static TvAudio GetTvAudios(Program sdProgram)
    {
        TvAudio ret = new();

        if (sdProgram.AudioProperties != null && sdProgram.AudioProperties.Any())
        {
            List<string> a = sdProgram.AudioProperties.ToList();
            if (a.Any())
            {
                ret.Stereo = a[0];
            }

        }

        return ret;

    }

    public async Task<string> GetEpg(CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
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

                        Title = GetTitles(sdProg.Titles, lang),
                        Subtitle = GetSubTitles(sdProg, lang),
                        Desc = GetDescriptions(sdProg, lang),
                        Credits = GetCredits(sdProg, lang),
                        Category = GetCategory(sdProg, lang),
                        Language = lang,
                        Episodenum = GetEpisodeNums(sdProg, lang),
                        Icon = GetIcons(p, sdProg, sched, lang),
                        Rating = GetRatings(sdProg, lang, setting.SDMaxRatings),
                        Video = GetTvVideos(p),
                        Audio = GetTvAudios(p),

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
