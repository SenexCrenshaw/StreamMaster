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
    private readonly object fileLock = new();
    public static readonly int MAX_RETRIES = 2;
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly ILogger _logger = logger;
    private readonly string ImageInfoFilePath = Path.Combine(BuildInfo.SDImagesFolder, "ImageInfo.json");

    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        return await GetData<Countries>("available/countries", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        return await GetData<List<Headend>>($"headends?country={country}&postalcode={postalCode}", cancellationToken).ConfigureAwait(false);
    }

    public async Task ProcessProgramsImages(List<SDProgram> sDPrograms, CancellationToken cancellationToken)
    {
        List<string> programIds = sDPrograms
            .Where(a=>  a.HasImageArtwork==true || a.HasSportsArtwork == true || a.HasSeriesArtwork == true || a.HasSeasonArtwork == true || a.HasMovieArtwork== true || a.HasEpisodeArtwork == true)
            .Select(a => a.ProgramID).Distinct().ToList();
        List<string> distinctProgramIds = programIds
                                             .Distinct()
                                             .Select(a => a.Length >= 10 ? a[..10] : a) // Select the leftmost 10 characters
                                             .ToList();

        if (programIds.Any())
        {
            List<ProgramMediaMetaData>? fetchedResults = await PostData<List<ProgramMediaMetaData>>("metadata/programs/", programIds, cancellationToken).ConfigureAwait(false);
            if (fetchedResults == null)
            {
                return;
            }

            int count = 0;

            foreach (ProgramMediaMetaData m in fetchedResults)
            {
                ++count;
                logger.LogInformation("Caching program icons for {count}/{totalCount} programs", count, fetchedResults.Count);
                SDProgram? sdProg = sDPrograms.Find(a => a.ProgramID == m.ProgramID);
                string cats = string.Join(',', m.ImageData.Select(a => a.Category).Distinct());
                string tiers = string.Join(',', m.ImageData.Select(a => a.Tier).Distinct());

                if (sdProg is null)
                {
                    continue;
                }

                if (sdProg.HasEpisodeArtwork == true)
                {
                    //List<ImageData> catEpisode = m.ImageData.Where(item => item.Tier == "Episode").ToList();
                    //await DownloadImages(m.ProgramID, catEpisode, cancellationToken);
                }

                if (sdProg.HasMovieArtwork == true)
                {
                    List<ImageData> iconsSports = m.ImageData.Where(item => item.Category.StartsWith("Poster")).ToList();
                    await DownloadImages(m.ProgramID, iconsSports, cancellationToken);
                    continue;
                }

                if (sdProg.HasSeasonArtwork == true)
                {
                    List<ImageData> catSeason = m.ImageData.Where(item => item.Tier == "Season").ToList();
                    await DownloadImages(m.ProgramID, catSeason, cancellationToken);
                }

                if (sdProg.HasSeriesArtwork == true)
                {
                    List<ImageData> catSeries = m.ImageData.Where(item => item.Tier == "Series").ToList();
                    await DownloadImages(m.ProgramID, catSeries, cancellationToken);
                }

                if (sdProg.HasSportsArtwork == true)
                {
                    List<ImageData> iconsSports = [.. m.ImageData];
                    await DownloadImages(m.ProgramID, iconsSports, cancellationToken);
                }

                if (sdProg.HasImageArtwork == true)
                {
                    await DownloadImages(m.ProgramID, m.ImageData, cancellationToken);
                }
            }
        }
    }

    private async Task DownloadImages(string programId, List<ImageData> iconsList, CancellationToken cancellationToken)
    {
        List<ImageData> icons = iconsList.Where(a => a.Category == "Banner-L1").ToList();
        if (!icons.Any())
        {
            icons = iconsList.Where(a => a.Category == "Iconic").ToList();
            if (!icons.Any())
            {
                icons = iconsList;
            }
        }

        icons = icons.Where(a => !string.IsNullOrEmpty(a.Uri) && a.Width <= 600 && a.Height <= 600).ToList();
        if (!icons.Any())
        {
            return;
        }

        logger.LogInformation("Downloading {count} icons for {ProgramID} program", icons.Count, programId);

        // Create a SemaphoreSlim for limiting concurrent downloads
        using SemaphoreSlim semaphore = new(4);
        List<Task<bool>> tasks = icons.ConvertAll(async icon =>
        {
            // Wait to enter the semaphore (limits the concurrency level)
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                // Perform the download task
                return await GetImageUrl(programId, icon, cancellationToken);
            }
            finally
            {
                // Release the semaphore
                semaphore.Release();
            }
        });

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);
    }

    private static string CleanUpFileName(string fullName)
    {
        // Remove double spaces, trim, and replace spaces with underscores
        fullName = string.Join("_", fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));

        // Ensure the file name doesn't start or end with an underscore
        if (fullName.StartsWith("_"))
        {
            fullName = fullName.TrimStart('_');
        }

        if (fullName.EndsWith("_"))
        {
            fullName = fullName.TrimEnd('_');
        }
        return fullName;
    }

    public async Task<bool> GetImageUrl(string programId, ImageData icon, CancellationToken cancellationToken)
    {
        List<ImageInfo> imageInfos = memoryCache.ImageInfos();

        if (File.Exists(ImageInfoFilePath) && !imageInfos.Any())
        {
            imageInfos = JsonSerializer.Deserialize<List<ImageInfo>>(File.ReadAllText(ImageInfoFilePath)) ?? [];
            memoryCache.SetCache(imageInfos);
        }

        if (imageInfos.Find(a => a.IconUri == icon.Uri) != null)
        {
            return true;
        }

        string fullName = Path.Combine(BuildInfo.SDImagesFolder, $"{programId}_{icon.Category}_{icon.Tier}_{icon.Width}x{icon.Height}.png");
        fullName = CleanUpFileName(fullName);

        try
        {
            string url = "";
            if (icon.Uri.StartsWith("http"))
            {
                url = icon.Uri;
            }
            else
            {
                url = await SdToken.GetAPIUrl($"image/{icon.Uri}", cancellationToken);
            }

            Setting setting = await settingsService.GetSettingsAsync(cancellationToken);
            _ = await EnsureToken(cancellationToken);

            HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
            using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
            {
                return false;
            }

            _ = response.EnsureSuccessStatusCode();

            Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            if (stream != null)
            {
                using FileStream fileStream = new(fullName, FileMode.Create);
                await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                WriteImageInfoToJsonFile(programId, icon, url, fullName);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to download image from {Url} to {FileName}.", icon.Uri, fullName);
        }
        return false;
    }
    private void WriteImageInfoToJsonFile(string programId, ImageData icon, string realUrl, string fullName)
    {
        lock (fileLock)
        {
            try
            {
                List<ImageInfo> imageInfos = memoryCache.ImageInfos();

                if (imageInfos.Find(a => a.IconUri == icon.Uri) != null)
                {
                    return;
                }

                UriBuilder uriBuilder = new(realUrl)
                {
                    // Remove all query parameters by setting the Query property to an empty string
                    Query = ""
                };

                // Get the modified URL
                string modifiedUrl = uriBuilder.Uri.ToString();

                // Add the new imageInfo
                imageInfos.Add(new ImageInfo(programId, icon.Uri, modifiedUrl, fullName, icon.Width, icon.Height));

                // Serialize the updated imageInfos to JSON and write it to the file using System.Text.Json
                string json = JsonSerializer.Serialize(imageInfos, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ImageInfoFilePath, json);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update image information in JSON file.");
            }
        }
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
        List<ChannelLogoDto> logoTasks =
        [
            .. setting.SDStationIds.AsParallel().WithCancellation(cancellationToken).SelectMany(SDStationId =>
                    {
                        return stationDictionary.TryGetValue(SDStationId.StationId, out Station? station) && station.Lineup == SDStationId.Lineup
                            ? station.StationLogo?.Where(logo => !channelLogos.Any(a => a.Source == logo.URL))
                                .Select(logo => new ChannelLogoDto
                                {
                                    Id = Interlocked.Increment(ref nextId),
                                    Source = logo.URL,
                                    Name = station.Name,
                                    EPGId = "SD|" + SDStationId.StationId,
                                    EPGFileId = 0
                                }) ?? Enumerable.Empty<ChannelLogoDto>()
                            : Enumerable.Empty<ChannelLogoDto>();
                    }),
        ];

        if (logoTasks.Count != 0)
        {
            _logger.LogInformation("SD working on {num} logos", logoTasks.Count);
        }

        logoTasks.ForEach(memoryCache.Add);

        List<Schedule>? schedules = await GetSchedules([.. stationsIds], cancellationToken).ConfigureAwait(false);
        if (schedules?.Any() != true)
        {
            return false;
        }

        List<Programme> programmes = [];
        DateTime now = DateTime.Now;
        DateTime maxDate = now.AddDays(maxDays);

        if (schedules.Count != 0)
        {
            _logger.LogInformation("SD working on {num} schedules", schedules.Count);
        }

        int totalPrograms = schedules.SelectMany(a => a.Programs).Count(p => p.AirDateTime <= maxDate);

        _logger.LogInformation("SD working on {num} programs", totalPrograms);
        int counter = 0;

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
            await ProcessProgramsImages(sdPrograms, cancellationToken).ConfigureAwait(false);

            Dictionary<string, SDProgram> sdProgramsDict = sdPrograms.ToDictionary(p => p.ProgramID);

            foreach (Program? p in relevantPrograms)
            {
                counter++;
                if (counter % 100 == 0)
                {
                    _logger.LogInformation("Processed {counter} programs out of {totalPrograms}", counter, totalPrograms);
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
                    //Name = channelNameSuffix,
                    DisplayName = displayName,
                    Title = SDHelpers.GetTitles(sdProg.Titles, lang),
                    Subtitle = SDHelpers.GetSubTitles(sdProg, lang),
                    Desc = SDHelpers.GetDescriptions(sdProg, lang),
                    Credits = SDHelpers.GetCredits(sdProg),
                    Category = SDHelpers.GetCategory(sdProg, lang),
                    Language = lang,
                    Episodenum = SDHelpers.GetEpisodeNums(sdProg),
                    Icon = SDHelpers.GetIcons(sdProg, memoryCache),
                    Rating = SDHelpers.GetRatings(sdProg, maxRatings),
                    Video = SDHelpers.GetTvVideos(p),
                    Audio = SDHelpers.GetTvAudios(p),
                };

                if (p.New != null)
                {
                    programme.New = "";
                }
                else
                {
                    programme.New = null;
                    programme.Previouslyshown = SDHelpers.GetPreviouslyShown(sdProg);
                }

                if (!string.IsNullOrEmpty(p.LiveTapeDelay) && p.LiveTapeDelay.Equals("Live"))
                {
                    programme.Live = "";
                }

                programmes.Add(programme);
            }
            _logger.LogInformation("Processed {counter} programs out of {totalPrograms}", counter, totalPrograms);
        }
        _logger.LogInformation("SD Finished");
        return memoryCache.SetSDProgreammesCache(programmes);
    }

    public async Task<LineupResult?> GetLineup(string lineup, CancellationToken cancellationToken)
    {
        return await GetData<LineupResult>($"lineups/{lineup}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<LineupPreview>> GetLineupPreviews(CancellationToken cancellationToken)
    {
        List<LineupPreview> res = [];
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
            return status.systemStatus.Any() && status.systemStatus[0].status?.ToLower() == "online";
        }
        catch
        {
            return false;
        }
    }

    public async Task<SDStatus> GetStatus(CancellationToken cancellationToken)
    {
        SDStatus status = await GetStatusInternal(cancellationToken);
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
        if (result == null)
        {
            return SDHelpers.GetSDStatusOffline();
        }
        result = await HandleStatus(result, cancellationToken).ConfigureAwait(false);

        return result ?? SDHelpers.GetSDStatusOffline();
    }

    private async Task<SDStatus?> HandleStatus(SDStatus sdstatus, CancellationToken cancellationToken)
    {
        if ((SDHttpResponseCode)sdstatus.code is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED or SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
        {
            if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
            {
                return null;
            }
            ResetCache(SDCommands.Status);
            return await GetStatusInternal(cancellationToken).ConfigureAwait(false);
        }
        return sdstatus;
    }

    private async Task<string?> EnsureToken(CancellationToken cancellationToken)
    {
        return await SdToken.GetTokenAsync(cancellationToken);
    }

    private async Task<T?> GetData<T>(string command, CancellationToken cancellationToken, bool dontCache = false)
    {
        string cacheKey = SDHelpers.GenerateCacheKey(command);
        string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);

        if (!dontCache)
        {
            // Check if cache exists and is valid
            TimeSpan duration = CacheDuration;
            if (command == SDCommands.Status || command == SDCommands.LineUps)
            {
                duration = TimeSpan.FromMinutes(5);
            }

            string? token = await EnsureToken(cancellationToken);
            if (!string.IsNullOrEmpty(token) && File.Exists(cachePath) && DateTime.UtcNow - File.GetLastWriteTimeUtc(cachePath) <= duration)
            {
                string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken);
                SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);

                if (cacheEntry != null && DateTime.UtcNow - cacheEntry.Timestamp <= duration)
                {
                    return cacheEntry.Data;
                }
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

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, logger,cancellationToken).ConfigureAwait(false);

                if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
                    return default;
                }

                if (responseCode is SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
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

                if (!dontCache)
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

    private async Task<T?> PostData<T>(string command, object toPost, CancellationToken cancellationToken)
    {
        string jsonString = JsonSerializer.Serialize(toPost);

        StringContent content = new(jsonString, Encoding.UTF8, "application/json");
        string? responseContent = "";
        int retry = 0;
        try
        {
            while (retry <= MAX_RETRIES)
            {
                ++retry;
                string? url = await SdToken.GetAPIUrl(command, cancellationToken);
                Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

                HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
                using HttpResponseMessage response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, logger, cancellationToken).ConfigureAwait(false);

                if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
                    return default;
                }

                if (responseCode is SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }

                    continue;
                }

                return result == null ? default : result;
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"Exception cannot deserialize data for {command} to {typeof(T).Name}");
            Console.WriteLine(responseContent);
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
                using HttpResponseMessage response = await httpClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, PutResponse? result) = await SDHandler.ProcessResponse<PutResponse?>(response, logger, cancellationToken).ConfigureAwait(false);

                if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
                    return default;
                }

                if (responseCode is SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }
                    ++retry;
                    continue;
                }

                return result ?? default;
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
                using HttpResponseMessage response = await httpClient.PutAsync(url, null, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, PutResponse? result) = await SDHandler.ProcessResponse<PutResponse?>(response, logger, cancellationToken).ConfigureAwait(false);

                if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
                    return default;
                }

                if (responseCode is SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
                {
                    if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }
                    ++retry;
                    continue;
                }

                return result ?? default;
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
            return [];
        }
        List<Lineup> lineups = res.Lineups.Where(a => !a.IsDeleted).ToList();
        return lineups;
    }

    public async Task<List<SDProgram>> GetSDPrograms(List<string> programIds, CancellationToken cancellationToken)
    {
        List<string> distinctProgramIds = programIds.Distinct().ToList();
        List<SDProgram> results = [];
        List<string> programIdsToFetch = [];

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
                return [];
            }

            HashSet<string> processedProgramIds = [];

            foreach (SDProgram program in fetchedResults)
            {
                // Check if we've already processed this ProgramID
                if (!processedProgramIds.Contains(program.ProgramID))
                {
                    // Write to cache if it's a new ProgramID
                    await WriteToCacheAsync("Program_" + program.ProgramID, program, cancellationToken).ConfigureAwait(false);
                    results.Add(program);
                    // Mark this ProgramID as processed
                    _ = processedProgramIds.Add(program.ProgramID);
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
            _ = _cacheSemaphore.Release();
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

            return cacheEntry != null && DateTime.Now - cacheEntry.Timestamp <= CacheDuration ? cacheEntry.Data : default;
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    public async Task<bool> AddLineup(string lineup, CancellationToken cancellationToken)
    {
        PutResponse? fetchedResults = await PutData($"lineups/{lineup}", cancellationToken).ConfigureAwait(false);
        return fetchedResults != null;
    }

    public async Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken)
    {
        PutResponse? fetchedResults = await DeleteData($"lineups/{lineup}", cancellationToken).ConfigureAwait(false);
        return fetchedResults != null;
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
            return [];
        }
        List<StationPreview> ret = [];
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
        List<Station> ret = [];

        List<Lineup> lineups = await GetLineups(cancellationToken).ConfigureAwait(false);
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
                    _ = existingIds.Add(station.StationId);
                }
            }
        }

        return ret;
    }
}