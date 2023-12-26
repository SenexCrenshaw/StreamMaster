using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;

using StreamMaster.Domain.Common;

using System.Diagnostics;
using System.Text.RegularExpressions;


namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirect
{
    private static List<KeyValuePair<MxfService, string[]>> StationLogosToDownload = [];

    private async Task<bool> BuildLineupServices(CancellationToken cancellationToken = default)
    {
        LineupResponse? clientLineups = await GetSubscribedLineups(cancellationToken).ConfigureAwait(false);
        if (clientLineups == null || !clientLineups.Lineups.Any())
        {
            return false;
        }

        Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
        string preferredLogoStyle = string.IsNullOrEmpty(setting.SDSettings.PreferredLogoStyle) ? "DARK" : setting.SDSettings.PreferredLogoStyle;
        string alternateLogoStyle = string.IsNullOrEmpty(setting.SDSettings.AlternateLogoStyle) ? "WHITE" : setting.SDSettings.AlternateLogoStyle;
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.GetSchedulesDirectData(0);

        foreach (SubscribedLineup clientLineup in clientLineups.Lineups)
        {
            // don't download station map if lineup not included
            if (setting.SDSettings.SDStationIds.FirstOrDefault(a => a.Lineup == clientLineup.Lineup) == null)
            {
                //logger.LogWarning($"Subscribed lineup {clientLineup.Lineup} has been EXCLUDED by user from download and processing.");
                continue;
            }

            if (clientLineup.IsDeleted)
            {
                logger.LogWarning($"Subscribed lineup {clientLineup.Lineup} has been DELETED at the headend.");
                continue;
            }


            // get/create lineup
            MxfLineup mxfLineup = schedulesDirectData.FindOrCreateLineup(clientLineup.Lineup, $"SM {clientLineup.Name} ({clientLineup.Location})");

            Debug.Assert(!mxfLineup.extras.ContainsKey("epgid"));

            // request the lineup's station maps
            StationChannelMap? lineupMap = await GetStationChannelMap(clientLineup.Lineup, cancellationToken);
            if (lineupMap == null || ((lineupMap?.Stations?.Count ?? 0) == 0))
            {
                logger.LogError($"Subscribed lineup {clientLineup.Lineup} does not contain any stations.");
                return false;
            }

            // use hashset to make sure we don't duplicate channel entries for this station
            HashSet<string> channelNumbers = [];

            // build the services and lineup
            foreach (LineupStation station in lineupMap.Stations)
            {
                // check if station should be downloaded and processed
                if (station == null || setting.SDSettings.SDStationIds.FirstOrDefault(a => a.StationId == station.StationId) == null)
                {
                    continue;
                }

                // build the service if necessary
                MxfService mxfService = schedulesDirectData.FindOrCreateService(station.StationId);
                Debug.Assert(!mxfService.extras.ContainsKey("epgid"));
                if (string.IsNullOrEmpty(mxfService.CallSign))
                {
                    // instantiate stationLogo and override uid
                    StationImage? stationLogo = null;
                    mxfService.UidOverride = $"!Service!STREAMMASTER_{station.StationId}";

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
                        stationLogo = station.StationLogos?.FirstOrDefault(arg => arg.Category != null && arg.Category.Equals(preferredLogoStyle, StringComparison.OrdinalIgnoreCase)) ??
                                      station.StationLogos?.FirstOrDefault(arg => arg.Category != null && arg.Category.Equals(alternateLogoStyle, StringComparison.OrdinalIgnoreCase)) ??
                                      station.Logo;

                        // initialize as custom logo
                        string logoPath = string.Empty;
                        string urlLogoPath = string.Empty;

                        string logoFilename = $"{station.Callsign}_c.png";
                        if (File.Exists($"{BuildInfo.SDStationLogos}{logoFilename}"))
                        {
                            logoPath = $"{BuildInfo.SDStationLogos}{logoFilename}";
                            urlLogoPath = stationLogo.Url;
                        }
                        else if (stationLogo != null)
                        {
                            logoFilename = $"{stationLogo.Md5}.png";
                            logoPath = $"{BuildInfo.SDStationLogosCache}{logoFilename}";
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
                                using FileStream stream = File.OpenRead(logoPath);
                                using Image image = await Image.LoadAsync(stream, cancellationToken);

                                mxfService.extras.Add("logo", new StationImage
                                {
                                    Url = urlLogoPath,
                                    Height = image.Height,
                                    Width = image.Width
                                });
                            }
                            else if (stationLogo != null)
                            {
                                mxfService.extras.Add("logo", new StationImage
                                {
                                    Url = urlLogoPath
                                });
                            }
                        }
                    }
                }

                // match station with mapping for lineup number and subnumbers
                foreach (LineupChannel map in lineupMap.Map)
                {
                    if (!map.StationId.Equals(station.StationId))
                    {
                        continue;
                    }

                    int number = map.myChannelNumber;
                    int subnumber = map.myChannelSubnumber;

                    string matchName = map.ProviderCallsign;
                    switch (clientLineup.Transport)
                    {
                        case "Satellite":
                        case "DVB-S":
                            Match m = Regex.Match(lineupMap.Metadata.Lineup, @"\d+\.\d+");
                            if (m.Success && map.FrequencyHz > 0 && map.NetworkId > 0 && map.TransportId > 0 && map.ServiceId > 0)
                            {
                                while (map.FrequencyHz > 13000)
                                {
                                    map.FrequencyHz /= 1000;
                                }
                                matchName = $"DVBS:{m.Value.Replace(".", "")}:{map.FrequencyHz}:{map.NetworkId}:{map.TransportId}:{map.ServiceId}";
                                number = -1;
                                subnumber = 0;
                            }
                            break;
                        case "Antenna":
                        case "DVB-T":
                            if (map.NetworkId > 0 && map.TransportId > 0 && map.ServiceId > 0)
                            {
                                matchName = $"DVBT:{map.NetworkId}:{map.TransportId}:{map.ServiceId}";
                                break;
                            }
                            if (map.AtscMajor > 0 && map.AtscMinor > 0)
                            {
                                matchName = $"OC:{map.AtscMajor}:{map.AtscMinor}";
                            }
                            break;
                    }

                    //if (config.DiscardChanNumbers.Contains(clientLineup.Lineup))
                    //{
                    //    number = -1; subnumber = 0;
                    //}

                    string channelNumber = $"{number}{(subnumber > 0 ? $".{subnumber}" : "")}";
                    if (channelNumbers.Add($"{channelNumber}:{station.StationId}"))
                    {
                        mxfLineup.channels.Add(new MxfChannel(mxfLineup, mxfService, number, subnumber)
                        {
                            MatchName = matchName
                        });
                    }
                }
            }
        }

        if (StationLogosToDownload.Count > 0)
        {
            logger.LogInformation($"Kicking off background worker to download and process {StationLogosToDownload.Count} station logos.");
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


        if (schedulesDirectData.Services.Count > 0)
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


            UpdateIcons(schedulesDirectData.Services);



            logger.LogInformation("Exiting BuildLineupServices(). SUCCESS.");
            return true;
        }

        logger.LogError($"There are 0 stations queued for download from {clientLineups.Lineups.Count} subscribed lineups. Exiting.");
        return false;
    }
}
