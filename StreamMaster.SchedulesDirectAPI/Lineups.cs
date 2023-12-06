using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;

using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;


namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private static List<KeyValuePair<MxfService, string[]>> StationLogosToDownload = [];
    private static volatile bool StationLogosDownloadComplete = true;

    private async Task<bool> BuildLineupServices(CancellationToken cancellationToken = default)
    {
        var clientLineups = await GetSubscribedLineups(cancellationToken).ConfigureAwait(false);
        if (clientLineups == null || !clientLineups.Lineups.Any()) return false;

        Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
       
        foreach (var clientLineup in clientLineups.Lineups)
        {
            // don't download station map if lineup not included
            if (setting.SDSettings.SDStationIds.FirstOrDefault(a => a.Lineup == clientLineup.Lineup) == null)
            {
                logger.LogWarning($"Subscribed lineup {clientLineup.Lineup} has been EXCLUDED by user from download and processing.");
                continue;
            }

            if (clientLineup.IsDeleted)
            {
                logger.LogWarning($"Subscribed lineup {clientLineup.Lineup} has been DELETED at the headend.");
                continue;
            }

            // get/create lineup
            var mxfLineup = schedulesDirectData.FindOrCreateLineup(clientLineup.Lineup, $"EPG123 {clientLineup.Name} ({clientLineup.Location})");


            // request the lineup's station maps
            var lineupMap = await GetStationChannelMap(clientLineup.Lineup);
            if (lineupMap == null || ((lineupMap?.Stations?.Count ?? 0) == 0))
            {
                logger.LogError($"Subscribed lineup {clientLineup.Lineup} does not contain any stations.");
                return false;
            }

            // use hashset to make sure we don't duplicate channel entries for this station
            var channelNumbers = new HashSet<string>();

            // build the services and lineup
            foreach (var station in lineupMap.Stations)
            {
                // check if station should be downloaded and processed
                if (station == null || setting.SDSettings.SDStationIds.FirstOrDefault(a => a.StationId == station.StationId) == null) continue;
               
                // build the service if necessary
                var mxfService = schedulesDirectData.FindOrCreateService(station.StationId);
                if (string.IsNullOrEmpty(mxfService.CallSign))
                {
                    // instantiate stationLogo and override uid
                    StationImage? stationLogo = null;
                    mxfService.UidOverride = $"!Service!EPG123_{station.StationId}";

                    // add callsign and station name
                    mxfService.CallSign =station.Callsign;
                    if (string.IsNullOrEmpty(mxfService.Name))
                    {
                        var names = Regex.Matches(station.Name.Replace("-", ""), station.Callsign);
                        if (names.Count > 0)
                        {
                            mxfService.Name = (!string.IsNullOrEmpty(station.Affiliate) ? $"{station.Name} ({station.Affiliate})" : station.Name);
                        }
                        else mxfService.Name = station.Name;
                    }

                    // add affiliate if available
                    if (!string.IsNullOrEmpty(station.Affiliate))
                    {
                        mxfService.mxfAffiliate = schedulesDirectData.FindOrCreateAffiliate(station.Affiliate);
                    }

                    // add station logo if available
                    if (station.StationLogos != null) {
                        var cats = station.StationLogos.Select(a => a.Category).Distinct().ToList();
                        stationLogo = station.StationLogos?.FirstOrDefault(arg => arg.Category != null && arg.Category.Equals("DARK", StringComparison.OrdinalIgnoreCase)) ??
                                      station.StationLogos?.FirstOrDefault(arg => arg.Category != null && arg.Category.Equals("WHITE", StringComparison.OrdinalIgnoreCase)) ??
                                      station.Logo;
                   
                    // initialize as custom logo
                    var logoPath = string.Empty;
                    var urlLogoPath = string.Empty;
        
                    var logoFilename = $"{station.Callsign}_c.png";
                    if ( File.Exists($"{BuildInfo.SDStationLogos}{logoFilename}"))
                    {
                        logoPath = $"{BuildInfo.SDStationLogos}{logoFilename}";
                        urlLogoPath = stationLogo.Url;                        
                    }
                    else if (stationLogo != null)
                    {
                        logoFilename = $"{stationLogo.Md5}.png";
                        logoPath = $"{BuildInfo.SDStationLogosCache}{logoFilename}";
                        urlLogoPath = stationLogo.Url;

                        if ( !File.Exists(logoPath))
                        {
                            StationLogosToDownload.Add(new KeyValuePair<MxfService, string[]>(mxfService, [logoPath, stationLogo.Url]));
                        }
                       
                    }

                    // add to mxf guide images if file exists already
                    if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                    {
                        mxfService.mxfGuideImage = schedulesDirectData.FindOrCreateGuideImage( urlLogoPath);
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
                            using var stream = File.OpenRead(logoPath);
                            using var image = await Image.LoadAsync(stream, cancellationToken);

                            mxfService.extras.Add("logo", new StationImage
                            {
                                Url =  urlLogoPath,
                                Height = image.Height,
                                Width = image.Width
                            });
                        }
                        else if (stationLogo != null)
                        {
                            mxfService.extras.Add("logo", new StationImage
                            {
                                Url =  urlLogoPath
                            });
                        }
                        }
                    }
                }

                // match station with mapping for lineup number and subnumbers
                foreach (var map in lineupMap.Map)
                {
                    if (!map.StationId.Equals(station.StationId)) continue;
                    var number = map.myChannelNumber;
                    var subnumber = map.myChannelSubnumber;

                    string matchName = map.ProviderCallsign;
                    switch (clientLineup.Transport)
                    {
                        case "Satellite":
                        case "DVB-S":
                            var m = Regex.Match(lineupMap.Metadata.Lineup, @"\d+\.\d+");
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

                    var channelNumber = $"{number}{(subnumber > 0 ? $".{subnumber}" : "")}";
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
            StationLogosDownloadComplete = false;
            logger.LogInformation($"Kicking off background worker to download and process {StationLogosToDownload.Count} station logos.");
            await Task.Run(async () =>
            {
                try
                {
                    await DownloadStationLogos(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while downloading station logos.");
                }
            });
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
        logger.LogError("Check that lineups are 'INCLUDED' and stations are selected in the EPG123 GUI.");
        return false;        
    }

    private async Task<bool> DownloadStationLogos(CancellationToken cancellationToken)
    {
        Setting setting = memoryCache.GetSetting();
        if (!setting.SDSettings.SDEnabled)
        {
            return false;
        }

        if (StationLogosToDownload.Count == 0)
        {
            return false;
        }

        int maxConcurrentDownloads = 4;
        var semaphore = new SemaphoreSlim(maxConcurrentDownloads);

        var tasks = StationLogosToDownload.Select(async serviceLogo =>
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                var logoPath = serviceLogo.Value[0];
                if ((File.Exists(logoPath) || await DownloadSdLogo(serviceLogo.Value[1], logoPath, cancellationToken)) && string.IsNullOrEmpty(serviceLogo.Key.LogoImage))
                {
                    
                    serviceLogo.Key.mxfGuideImage = schedulesDirectData.FindOrCreateGuideImage(logoPath);

                    if (File.Exists(logoPath))
                    {
                        // update dimensions
                        using var stream = File.OpenRead(logoPath);
                        using var image = await Image.LoadAsync(stream, cancellationToken);
                        serviceLogo.Key.extras["logo"].Height = image.Height;
                        serviceLogo.Key.extras["logo"].Width = image.Width;
                        StationLogosToDownload.Remove(serviceLogo);
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }).ToArray();

        await Task.WhenAll(tasks);

        return true;
    }

    private async Task<Image<Rgba32>> CropAndResizeImageAsync(Stream stream)
    {
        try
        {
            //using var stream = File.OpenRead(imagePath);
            using var image = await Image.LoadAsync<Rgba32>(stream);

            const int tgtWidth = 360;
            const int tgtHeight = 270;
            const double tgtAspect = 3.0;

            // Find the min/max non-transparent pixels
            var min = new Size(int.MaxValue, int.MaxValue);
            var max = new Size(int.MinValue, int.MinValue);

            for (var x = 0; x < image.Width; ++x)
            {
                for (var y = 0; y < image.Height; ++y)
                {
                    var pixelColor = image[x, y];
                    if (pixelColor.A <= 0) continue;
                    if (x < min.Width) min.Width = x;
                    if (y < min.Height) min.Height = y;

                    if (x > max.Width) max.Width = x;
                    if (y > max.Height) max.Height = y;
                }
            }

            // Create a new image with the crop and resize
            var cropRectangle = new Rectangle(min.Width, min.Height, max.Width - min.Width + 1, max.Height - min.Height + 1);
            var croppedImage = image.Clone(ctx => ctx.Crop(cropRectangle));

            if ((max.Width - min.Width + 1) / tgtAspect > (max.Height - min.Height + 1))
            {
                var offsetY = (int)((max.Width - min.Width + 1) / tgtAspect - (max.Height - min.Height + 1) + 0.5) / 2;
                var newHeight = croppedImage.Height + offsetY * 2;

                var resizedImage = croppedImage.Clone(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(tgtWidth, newHeight),
                    Mode = ResizeMode.Max
                }));

                return resizedImage;
            }
            else
            {
                return croppedImage;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while cropping and resizing image.");
            throw;
        }
    }
}
