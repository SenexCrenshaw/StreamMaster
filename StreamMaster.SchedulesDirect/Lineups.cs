using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using StreamMaster.Domain.Helpers;

using System.Text.RegularExpressions;
namespace StreamMaster.SchedulesDirect;
public class Lineups(ILogger<Lineups> logger, IOptionsMonitor<SDSettings> intSDSettings, ILogoService logoService, ISchedulesDirectAPIService schedulesDirectAPI, IEPGCache<LineupResult> epgCache, ISchedulesDirectDataService schedulesDirectDataService)
    : ILineups
{
    private List<KeyValuePair<MxfService, string[]>> StationLogosToDownload = [];

    public void ResetCache()
    {
        StationLogosToDownload = [];
    }

    public async Task<bool> BuildLineupServices(CancellationToken cancellationToken = default)
    {
        SDSettings sdSettings = intSDSettings.CurrentValue;

        LineupResponse? clientLineups = await GetSubscribedLineups(cancellationToken).ConfigureAwait(false);

        if (clientLineups == null || clientLineups.Lineups.Count < 1)
        {
            return false;
        }

        string preferredLogoStyle = string.IsNullOrEmpty(sdSettings.PreferredLogoStyle) ? "DARK" : sdSettings.PreferredLogoStyle;
        string alternateLogoStyle = string.IsNullOrEmpty(sdSettings.AlternateLogoStyle) ? "WHITE" : sdSettings.AlternateLogoStyle;
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        foreach (SubscribedLineup clientLineup in clientLineups.Lineups)
        {
            // don't download station map if lineup not included
            if (sdSettings.SDStationIds.Find(a => a.Lineup == clientLineup.Lineup) == null)
            {
                //StationIdLineup stationIdLineup = new() { Id = $"{clientLineup.Id}|{clientLineup.Lineup}", Lineup = clientLineup.Lineup };
                //sdSettings.SDStationIds.Add(stationIdLineup);
                //SettingsHelper.UpdateSetting(sdSettings);
                //logger.LogWarning($"Subscribed lineup {clientLineup.Lineup} has been EXCLUDED by user from download and processing.");
                continue;
            }

            if (clientLineup.IsDeleted)
            {
                logger.LogWarning("Subscribed lineup {clientLineup.Lineup} has been DELETED at the headend.", clientLineup.Lineup);
                continue;
            }

            // get/create lineup
            MxfLineup mxfLineup = schedulesDirectData.FindOrCreateLineup(clientLineup.Lineup, $"SM {clientLineup.Name} ({clientLineup.Location})");

            // request the lineup's station maps
            StationChannelMap? lineupMap = await GetStationChannelMap(clientLineup.Lineup);
            if (lineupMap == null || ((lineupMap?.Stations?.Count ?? 0) == 0))
            {
                logger.LogError("Subscribed lineup {clientLineup.Lineup} does not contain any stations.", clientLineup.Lineup);
                //return false;
                continue;
            }

            // use hashset to make sure we don't duplicate channel entries for this station
            ConcurrentHashSet<string> channelNumbers = [];

            // build the services and lineup
            foreach (LineupStation station in lineupMap!.Stations)
            {
                // check if station should be downloaded and processed
                if (station == null || sdSettings.SDStationIds.Find(a => a.StationId == station.StationId) == null)
                {
                    continue;
                }

                // build the service if necessary
                string serviceName = $"{EPGHelper.SchedulesDirectId}-{station.StationId}";

                MxfService mxfService = schedulesDirectData.FindOrCreateService(serviceName);

                if (string.IsNullOrEmpty(mxfService.CallSign))
                {
                    // instantiate stationLogo and override uid
                    StationImage? stationLogo = null;
                    mxfService.UidOverride = $"!Service!STREAMMASTER_{serviceName}";

                    // add callsign and station name
                    mxfService.CallSign = station.Callsign;
                    if (string.IsNullOrEmpty(mxfService.Name))
                    {
                        MatchCollection names = Regex.Matches(station.Name.Replace("-", ""), station.Callsign);
                        mxfService.Name = names.Count > 0
                            ? !string.IsNullOrEmpty(station.Affiliate) ? $"{station.Name} ({station.Affiliate})" : station.Name
                            : station.Name;
                    }

                    // add affiliate if available
                    if (!string.IsNullOrEmpty(station.Affiliate))
                    {
                        mxfService.mxfAffiliate = schedulesDirectData.FindOrCreateAffiliate(station.Affiliate);
                    }

                    // add station logo if available
                    if (station.StationLogos != null)
                    {
                        List<string> cats = station.StationLogos.Select(a => a.Category).Distinct().ToList();
                        stationLogo = station.StationLogos?.FirstOrDefault(arg => arg.Category?.Equals(preferredLogoStyle, StringComparison.OrdinalIgnoreCase) == true) ??
                                      station.StationLogos?.FirstOrDefault(arg => arg.Category?.Equals(alternateLogoStyle, StringComparison.OrdinalIgnoreCase) == true) ??
                                      station.Logo;

                        // initialize as custom logo
                        string logoPath = string.Empty;
                        string urlLogoPath = string.Empty;

                        string logoFilename = $"{station.Callsign}_c.png";
                        string file = Path.Combine(BuildInfo.SDStationLogosFolder, logoFilename);
                        if (File.Exists(file))
                        {
                            logoPath = file;
                            urlLogoPath = stationLogo.Url;
                        }
                        else if (stationLogo != null)
                        {
                            logoFilename = $"{stationLogo.Md5}.png";
                            logoPath = Path.Combine(BuildInfo.SDStationLogosCacheFolder, logoFilename);
                            urlLogoPath = stationLogo.Url;

                            if (!File.Exists(logoPath))
                            {
                                StationLogosToDownload.Add(new KeyValuePair<MxfService, string[]>(mxfService, [logoPath, stationLogo.Url]));
                            }
                        }

                        // add to mxf guide images if file exists already
                        if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                        {
                            mxfService.mxfGuideImage = schedulesDirectData.FindOrCreateGuideImage(urlLogoPath);
                        }

                        // handle xmltv logos
                        //if (config.XmltvIncludeChannelLogos.Equals("url") && stationLogo != null)
                        //{
                        //    mxfService.extras.Add("logo", stationLogo);
                        //}
                        //else 
                        if (stationLogo != null)
                        {
                            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                            {
                                await using FileStream stream = File.OpenRead(logoPath);
                                using Image image = await Image.LoadAsync(stream, cancellationToken);

                                mxfService.extras.TryAdd("logo", new StationImage
                                {
                                    Url = urlLogoPath,
                                    Height = image.Height,
                                    Width = image.Width
                                });
                            }
                            else if (stationLogo != null)
                            {
                                mxfService.extras.TryAdd("logo", new StationImage
                                {
                                    Url = urlLogoPath
                                });
                            }
                        }
                    }
                }

                // match station with mapping for lineup number and subnumbers
                foreach (LineupChannelStation map in lineupMap.Map.Where(a => a.StationId == station.StationId))
                {
                    //if (!map.StationId.Equals(station.StationId))
                    //{
                    //    continue;
                    //}

                    //int number = map.myChannelNumber;
                    //int subnumber = map.myChannelSubnumber;

                    //string matchName = map.ProviderCallsign;
                    //switch (clientLineup.Transport)
                    //{
                    //    case "Satellite":
                    //    case "DVB-S":
                    //        Match m = Regex.Match(lineupMap.Metadata.Lineup, @"\d+\.\d+");
                    //        if (m.Success && map.FrequencyHz > 0 && map.NetworkId > 0 && map.TransportId > 0 && map.ServiceId > 0)
                    //        {
                    //            while (map.FrequencyHz > 13000)
                    //            {
                    //                map.FrequencyHz /= 1000;
                    //            }
                    //            matchName = $"DVBS:{m.Value.Replace(".", "")}:{map.FrequencyHz}:{map.NetworkId}:{map.TransportId}:{map.ServiceId}";
                    //            number = -1;
                    //            subnumber = 0;
                    //        }
                    //        break;
                    //    case "Antenna":
                    //    case "DVB-T":
                    //        if (map.NetworkId > 0 && map.TransportId > 0 && map.ServiceId > 0)
                    //        {
                    //            matchName = $"DVBT:{map.NetworkId}:{map.TransportId}:{map.ServiceId}";
                    //            break;
                    //        }
                    //        if (map.AtscMajor > 0 && map.AtscMinor > 0)
                    //        {
                    //            matchName = $"OC:{map.AtscMajor}:{map.AtscMinor}";
                    //        }
                    //        break;
                    //}

                    //if (config.DiscardChanNumbers.Contains(clientLineup.Lineup))
                    //{
                    //    number = -1; subnumber = 0;
                    //}

                    const string channelNumber = "1";// $"{number}{(subnumber > 0 ? $".{subnumber}" : "")}";
                    if (channelNumbers.Add($"{channelNumber}:{station.StationId}"))
                    {
                        mxfLineup.channels.Add(new MxfChannel(mxfLineup, mxfService)
                        {
                            MatchName = ""
                        });
                    }
                }
            }
        }

        if (StationLogosToDownload.Count > 0)
        {
            logger.LogInformation("Starting background worker to download and process {StationLogosToDownload.Count} station logos.", StationLogosToDownload.Count);
            await Task.Run(async () =>
            {
                try
                {
                    _ = await DownloadStationLogos(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while downloading station logos.");
                }
            }, cancellationToken);
        }

        if (!schedulesDirectData.Services.IsEmpty)
        {
            //// report specific stations that are no longer available
            //var missing = (from station in IncludedStations where schedulesDirectData.Services.FirstOrDefault(arg => arg.StationId.Equals(station)) == null select config.StationId.Single(arg => arg.StationId.Equals(station)).CallSign).ToList();
            //if (missing.Count > 0)
            //{
            //    MissingStations = missing.Count;
            //    Logger.WriteInformation($"Stations no longer available since last configuration save are: {string.Join(", ", missing)}");
            //}
            //var extras = mxf.With.Services.Where(arg => !IncludedStations.Contains(arg.StationId)).ToList();
            //if (extras.Count > 0)
            //{
            //    AddedStations = extras.Count;
            //    Logger.WriteInformation($"Stations added for download since last configuration save are: {string.Join(", ", extras.Select(e => e.CallSign))}");
            //}

            UpdateIcons(schedulesDirectData.Services.Values);

            logger.LogInformation("Exiting BuildLineupServices(). SUCCESS.");
            epgCache.SaveCache();
            return true;
        }

        logger.LogWarning("There are 0 stations queued for download from {LineupsCount} subscribed lineups. Exiting.", clientLineups.Lineups.Count);
        return false;
    }

    public async Task<List<StationChannelMap>> GetStationChannelMaps(CancellationToken cancellationToken)
    {
        List<StationChannelMap> ret = [];
        foreach (Station station in await GetStations(cancellationToken))
        {
            StationChannelMap? s = await GetStationChannelMap(station.Lineup);
            if (s is not null)
            {
                ret.Add(s);
            }
        }
        return ret;
    }

    private void UpdateIcons(ICollection<MxfService> Services)
    {
        foreach (MxfService? service in Services.Where(a => a.extras.ContainsKey("logo")))
        {
            StationImage artwork = service.extras["logo"];
            logoService.AddLogo(artwork.Url, service.CallSign);
        }
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
            Station station = stations[index];
            StationPreview sp = new(station);
            sp.Affiliate ??= "";
            ret.Add(sp);
        }
        return ret;
    }

    private async Task<StationChannelMap?> GetStationChannelMap(string lineup)
    {
        //StationChannelMap? cache = await GetValidCachedDataAsync<StationChannelMap>("StationChannelMap-" + lineup).ConfigureAwait(false);
        //if (cache != null)
        //{
        //    return cache;
        //}

        StationChannelMap? ret = await schedulesDirectAPI.GetApiResponse<StationChannelMap>(APIMethod.GET, $"lineups/{lineup}");
        if (ret != null)
        {
            //await WriteToCacheAsync("StationChannelMap-" + lineup, ret).ConfigureAwait(false);

            logger.LogDebug("Successfully retrieved the station mapping for lineup {lineup}. ({StationsCount} stations; {MapCount} channels)", lineup, ret.Stations.Count, ret.Map.Count);
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for retrieval of lineup {lineup}.", lineup);
        }

        return ret;
    }

    private async Task<LineupResponse?> GetSubscribedLineups(CancellationToken cancellationToken)
    {
        //LineupResponse? cache = await GetValidCachedDataAsync<LineupResponse>("SubscribedLineups", cancellationToken).ConfigureAwait(false);
        //if (cache != null)
        //{
        //    return cache;
        //}
        LineupResponse? cache = await schedulesDirectAPI.GetApiResponse<LineupResponse>(APIMethod.GET, "lineups", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            logger.LogDebug("Successfully requested listing of subscribed lineups from Schedules Direct.");
            //await WriteToCacheAsync("SubscribedLineups", cache, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for list of subscribed lineups.");
        }

        return cache;
    }

    public async Task<List<SubscribedLineup>> GetLineups(CancellationToken cancellationToken)
    {
        if (!intSDSettings.CurrentValue.SDEnabled)
        {
            return [];
        }

        LineupResponse? lineups = await GetSubscribedLineups(cancellationToken);

        return lineups is null ? ([]) : lineups.Lineups;
    }

    private async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        List<Station> ret = [];

        List<SubscribedLineup> lineups = await GetLineups(cancellationToken).ConfigureAwait(false);
        if (lineups.Count == 0)
        {
            return ret;
        }

        foreach (SubscribedLineup lineup in lineups)
        {
            LineupResult? res = await GetLineup(lineup.Lineup, cancellationToken).ConfigureAwait(false);
            if (res == null)
            {
                continue;
            }

            //foreach (Station station in res.Stations)
            //{
            //    station.Lineup = lineup.Lineup;
            //}

            ConcurrentHashSet<string> existingIds = new(ret.Select(station => station.StationId));

            foreach (Station station in res.Stations)
            {
                //var s = sdsettings.SDStationIds.FirstOrDefault(a => a.StationId == station.StationId && a.Lineup == lineup.Lineup);
                station.Lineup = lineup.Lineup;
                //if (s != null)
                //{
                //    station.Country = s.Country;
                //    station.PostalCode = s.PostalCode;
                //}
                //else
                //{

                //}

                if (!existingIds.Contains(station.StationId))
                {
                    ret.Add(station);
                    _ = existingIds.Add(station.StationId);
                }
            }
        }

        return ret;
    }
    private async Task<LineupResult?> GetLineup(string lineup, CancellationToken cancellationToken)
    {
        LineupResult? cache = await epgCache.GetValidCachedDataAsync("Lineup-" + lineup, cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            return cache;
        }
        cache = await schedulesDirectAPI.GetApiResponse<LineupResult>(APIMethod.GET, $"lineups/{lineup}", cancellationToken, cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            await epgCache.WriteToCacheAsync("Lineup-" + lineup, cache, cancellationToken).ConfigureAwait(false);
            logger.LogDebug("Successfully retrieved the channels in lineup {lineup}.", lineup);
            return cache;
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for retrieval of lineup {lineup}.", lineup);
        }
        return null;
    }

    private async Task<bool> DownloadStationLogos(CancellationToken cancellationToken)
    {
        SDSettings sdSettings = intSDSettings.CurrentValue;

        if (!sdSettings.SDEnabled)
        {
            return false;
        }

        if (StationLogosToDownload.Count == 0)
        {
            return false;
        }

        SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        Task[] tasks = StationLogosToDownload.Select(async serviceLogo =>
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                string logoPath = serviceLogo.Value[0];
                if (!File.Exists(logoPath))
                {
                    (int width, int height) = await DownloadSdLogoAsync(serviceLogo.Value[1], logoPath, cancellationToken).ConfigureAwait(false);
                    if (width == 0)
                    {
                        return;
                    }
                    serviceLogo.Key.mxfGuideImage = schedulesDirectData.FindOrCreateGuideImage(logoPath);

                    //if (File.Exists(logoPath))
                    //{
                    serviceLogo.Key.extras["logo"].Height = height;
                    serviceLogo.Key.extras["logo"].Width = width;
                    _ = StationLogosToDownload.Remove(serviceLogo);
                    //}
                }
            }
            finally
            {
                _ = semaphore.Release();
            }
        }).ToArray();

        await Task.WhenAll(tasks);

        return true;
    }

    private async Task<(int width, int height)> DownloadSdLogoAsync(string uri, string filePath, CancellationToken cancellationToken)
    {
        try
        {
            using HttpClient httpClient = new();
            HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                using Image<Rgba32> image = await Image.LoadAsync<Rgba32>(stream, cancellationToken).ConfigureAwait(false);
                if (image == null)
                {
                    return (0, 0);
                }
                using Image? cropImg = SDHelpers.CropAndResizeImage(image);
                if (cropImg == null)
                {
                    return (0, 0);
                }
                if (image!.Metadata.DecodedImageFormat == null)
                {
                    return (0, 0);
                }
                await using FileStream outputFileStream = File.Create(filePath);
                SixLabors.ImageSharp.Formats.IImageFormat? a = image.Metadata.DecodedImageFormat;
                cropImg.Save(outputFileStream, image.Metadata.DecodedImageFormat);
                return (cropImg.Width, cropImg.Height);
            }
            else
            {
                logger.LogError("HTTP request failed with status code: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("An exception occurred during DownloadSDLogoAsync(). Message:{ReportExceptionMessage}", FileUtil.ReportExceptionMessages(ex));
        }
        return (0, 0);
    }

    public void ClearCache()
    {
        epgCache.ResetCache();
    }
}
