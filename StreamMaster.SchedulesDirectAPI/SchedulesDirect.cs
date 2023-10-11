using StreamMaster.SchedulesDirectAPI.Models;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.EPG;
using StreamMasterDomain.Models;

using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public class SchedulesDirect
{
    private static HttpClient _httpClient;
    private readonly SDToken sdToken = null!;
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public SchedulesDirect(string clientUserAgent, string sdUserName, string sdPassword)
    {
        _httpClient = CreateHttpClient(clientUserAgent);
        sdToken = new SDToken(clientUserAgent, sdUserName, sdPassword);
    }

    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        Countries? result = await GetData<Countries>("available/countries", cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return null;

        }
        return result;

    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        //string? url = await sdToken.GetAPIUrl($"headends?country={country}&postalcode={postalCode}", cancellationToken);

        List<Headend>? result = await GetData<List<Headend>>($"headends?country={country}&postalcode={postalCode}", cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return null;

        }
        return result;

    }

    public async Task<bool> GetImageUrl(string source, string fileName, CancellationToken cancellationToken)
    {
        string? url = await sdToken.GetAPIUrl($"image/{source}", cancellationToken);
        if (url == null)
        {
            return false;
        }

        (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(url, fileName, cancellationToken).ConfigureAwait(false);

        return success;
    }

    public async Task<LineUpResult?> GetLineup(string lineUp, CancellationToken cancellationToken)
    {

        LineUpResult? result = await GetData<LineUpResult>($"lineups/{lineUp}", cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return null;

        }
        return result;

    }

    public async Task<List<LineUpPreview>> GetLineUpPreviews(CancellationToken cancellationToken)
    {
        List<LineUpPreview> res = new();
        LineUpsResult? lineups = await GetLineups(cancellationToken);

        if (lineups is null)
        {
            return res;
        }

        foreach (Lineup lineup in lineups.Lineups)
        {
            List<LineUpPreview>? results = await GetData<List<LineUpPreview>>($"lineups/preview/{lineup.LineupString}", cancellationToken).ConfigureAwait(false);

            if (results == null)
            {
                continue;
            }

            for (int index = 0; index < results.Count; index++)
            {
                LineUpPreview? lineUpPreview = results[index];
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


    private async Task<T?> GetData<T>(string command, CancellationToken cancellationToken)
    {
        string cacheKey = GenerateCacheKey(command);
        string cachePath = Path.Combine(BuildInfo.CacheFolder, "SD", cacheKey);

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
            while (retry <= SDToken.MAX_RETRIES)
            {
                string? url = await sdToken.GetAPIUrl(command, cancellationToken);
                using HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProccessResponse<T?>(response, cancellationToken).ConfigureAwait(false);

                if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    sdToken.LockOutToken();
                    return default;
                }

                if (responseCode == SDHttpResponseCode.TOKEN_EXPIRED || responseCode == SDHttpResponseCode.INVALID_USER)
                {
                    if (await sdToken.ResetToken(cancellationToken).ConfigureAwait(false) == null)
                    {
                        return default;
                    }
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
    private async Task<T?> PostData<T>(string command, StringContent stringContent, CancellationToken cancellationToken, bool ignoreCache = false)
    {
        string contentHash = GenerateHashFromStringContent(stringContent);
        string cacheKey = GenerateCacheKey($"{command}_{contentHash}");
        string cachePath = Path.Combine(BuildInfo.CacheFolder, "SD", cacheKey);


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
            while (retry <= SDToken.MAX_RETRIES)
            {
                string? url = await sdToken.GetAPIUrl(command, cancellationToken);
                using HttpResponseMessage response = await _httpClient.PostAsync(url, stringContent, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProccessResponse<T?>(response, cancellationToken).ConfigureAwait(false);

                if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
                {
                    sdToken.LockOutToken();
                    return default;
                }

                if (responseCode == SDHttpResponseCode.TOKEN_EXPIRED || responseCode == SDHttpResponseCode.INVALID_USER)
                {
                    if (await sdToken.ResetToken(cancellationToken).ConfigureAwait(false) == null)
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

    public async Task<LineUpsResult?> GetLineups(CancellationToken cancellationToken)
    {
        LineUpsResult? result = await GetData<LineUpsResult>("lineups", cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return null;
        }

        foreach (Lineup l in result.Lineups)
        {
            l.Id = l.LineupString;
        }

        return result;
    }

    public async Task<List<SDProgram>> GetSDPrograms(List<string> programIds, CancellationToken cancellationToken)
    {
        string jsonString = JsonSerializer.Serialize(programIds);
        StringContent content = new(jsonString, Encoding.UTF8, "application/json");

        List<SDProgram>? result = await PostData<List<SDProgram>>("programs", content, cancellationToken).ConfigureAwait(false);

        return result ?? new();

    }

    public async Task<List<Schedule>?> GetSchedules(List<string> stationIds, CancellationToken cancellationToken)
    {
        List<StationId> StationIds = new();
        foreach (string stationId in stationIds)
        {
            StationIds.Add(new StationId(stationId));
        }

        string jsonString = JsonSerializer.Serialize(StationIds);
        StringContent content = new(jsonString, Encoding.UTF8, "application/json");

        List<Schedule>? result = await PostData<List<Schedule>>("schedules", content, cancellationToken).ConfigureAwait(false);
        return result;

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
            Station? station = stations[index];
            StationPreview sp = new(station)
            {
                Id = index
            };
            ret.Add(sp);
        }
        return ret;
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        List<Station> ret = new();

        LineUpsResult? lineUps = await GetLineups(cancellationToken).ConfigureAwait(false);
        if (lineUps == null || lineUps.Lineups == null)
        {
            return ret;
        }

        foreach (Lineup lineUp in lineUps.Lineups)
        {
            LineUpResult? res = await GetLineup(lineUp.LineupString, cancellationToken).ConfigureAwait(false);
            if (res == null)
            {
                continue;
            }

            foreach (Station station in res.Stations)
            {
                station.LineUp = lineUp.LineupString;
            }
            ret.AddRange(res.Stations);
        }

        return ret;
    }

    public async Task<SDStatus?> GetStatus(CancellationToken cancellationToken)
    {
        return await sdToken.GetStatus(cancellationToken);
    }

    public async Task<bool> GetSystemReady(CancellationToken cancellationToken)
    {
        return await sdToken.GetSystemReady(cancellationToken);
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

    //public (string value, string lang) GetSubTtitle(string? subTitle)
    //{
    //    if (string.IsNullOrEmpty(subTitle))
    //    {
    //        return (string.Empty, string.Empty);
    //    }

    //    string[] parts = subTitle.Split(new char[] { ':' }, 2);
    //    if (parts.Length == 1)
    //    {
    //        return (parts[0], string.Empty);
    //    }
    //    return (parts[1], parts[0]);
    //}

    public static List<TvTitle> GetTitles(List<Title> Titles, string lang)
    {

        List<TvTitle> ret = Titles.Select(a => new TvTitle
        {
            Lang = lang,
            Text = a.Title120
        }).ToList();
        return ret;
    }

    public static TvSubtitle GetSubTitles(SDProgram sdProgram, string lang)
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
            Description100? test = sdProgram.Descriptions.Description100.FirstOrDefault(a => a.DescriptionLanguage == lang && a.Description != "");
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

    public static TvCredits GetCredits(SDProgram sdProgram, string lang)
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
            foreach (Cast? cast in sdProgram.Cast.OrderBy(a => a.BillingOrder))
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

    public static TvDesc GetDescriptions(SDProgram sdProgram, string lang)
    {

        string description = "";
        if (sdProgram.Descriptions is not null)
        {
            if (sdProgram.Descriptions.Description1000 is not null && sdProgram.Descriptions.Description1000.Any())
            {
                Description1000? test = sdProgram.Descriptions.Description1000.FirstOrDefault(a => a.DescriptionLanguage == lang && a.Description != "");
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

    public static List<TvCategory> GetCategory(SDProgram sdProgram, string lang)
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

    public static List<TvEpisodenum> GetEpisodeNums(SDProgram sdProgram, string lang)
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

    public static List<TvIcon> GetIcons(Program program, SDProgram sdProgram, Schedule sched, string lang)
    {
        List<TvIcon> ret = new();
        List<string> aspects = new() { "2x3", "4x3", "3x4", "16x9" };

        if (sdProgram.Metadata is not null && sdProgram.Metadata.Any())
        {

        }

        return ret;

    }

    public static List<TvRating> GetRatings(SDProgram sdProgram, string countryCode)
    {
        List<TvRating> ret = new();

        if (sdProgram.ContentRating != null && sdProgram.ContentRating.Any())
        {
            foreach (ContentRating cr in sdProgram.ContentRating)
            {
                ret.Add(new TvRating
                {
                    System = cr.Body,
                    Value = cr.Code
                }
                );
            }
        }

        return ret;

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

    public async Task<string> GetEpg(List<StationIdLineUp> stationIdLineUps, CancellationToken cancellationToken)
    {

        List<string> stationsIds = stationIdLineUps.Select(a => a.StationId).Distinct().ToList();

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

            List<string> names = stations.Where(a => a.StationID == stationId).Select(a => a.Name).Distinct().ToList();
            List<List<StationLogo>> logos = stations.Where(a => a.StationID == stationId).Select(a => a.StationLogo).Distinct().ToList();
            Logo? logo = stations.Where(a => a.StationID == stationId).Select(a => a.Logo).FirstOrDefault();
            Station station = stations.Where(a => a.StationID == stationId).First();

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
                Station station = stations.Where(a => a.StationID == sched.StationID).First();

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
                        Rating = GetRatings(sdProg, lang),
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
            //var s= GetSubTtitle()



        }

        Tv tv = new()
        {
            Channel = retChannels.OrderBy(a => int.Parse(a!.Id)).ToList(),
            Programme = retProgrammes.OrderBy(a => int.Parse(a.Channel)).ThenBy(a => a.StartDateTime).ToList()
        };


        return FileUtil.SerializeEpgData(tv);
    }

}
