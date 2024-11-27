using System.Collections.Concurrent;
using System.Globalization;

using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Crypto;
using StreamMaster.Domain.XML;

namespace StreamMaster.Application.EPG.Commands;

public class XMLTVBuilder(
    IOptionsMonitor<SDSettings> sdSettingsMonitor,
    IEPGService EPGService,
    ILogoService logoService,
    IFileUtilService fileUtilService,
    ICustomPlayListBuilder customPlayListBuilder,
    IHttpContextAccessor httpContextAccessor,
    ILogger<XMLTVBuilder> logger) : IXMLTVBuilder
{
    private readonly ConcurrentDictionary<int, XMLTV> xmlDict = new();

    public async Task<XMLTV?> CreateXmlTv(List<VideoStreamConfig> videoStreamConfigs)
    {
        xmlDict.Clear();
        try
        {
            XMLTV xmlTv = XMLUtil.NewXMLTV;

            await ProcessServicesAsync(xmlTv, videoStreamConfigs);

            xmlTv.SortXmlTv();

            return xmlTv;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to create the XMLTV file. Exception: {Message}", ex.Message);
            return null;
        }
        finally
        {
            xmlDict.Clear();
        }
    }


    public string GetUrlWithPath()
    {
        HttpRequest? request = httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return string.Empty;
        }

        string url = $"{request.Scheme}://{request.Host}";
        if (url.StartsWith("wss"))
        {
            url = "https" + url[3..];
        }
        return url;
    }

    private async Task ProcessServicesAsync(XMLTV xmlTv, List<VideoStreamConfig> videoStreamConfigs)
    {
        if (videoStreamConfigs == null || videoStreamConfigs.Count == 0)
        {
            return;
        }

        // Dictionary for quick lookup by EPGId
        Dictionary<string, VideoStreamConfig> videoStreamConfigDictionary = videoStreamConfigs.ToDictionary(a => a.EPGId);

        // Process Schedules Direct Configurations
        List<VideoStreamConfig> sdVideoStreamConfigs = videoStreamConfigs.Where(a => a.EPGNumber == EPGHelper.SchedulesDirectId).ToList();
        if (sdVideoStreamConfigs.Count > 0)
        {
            await ProcessScheduleDirectConfigsAsync(xmlTv, sdVideoStreamConfigs);
        }

        // Process Dummy Configurations
        List<VideoStreamConfig> dummyVideoStreamConfigs = videoStreamConfigs.Where(a => a.EPGNumber == EPGHelper.DummyId).ToList();
        if (dummyVideoStreamConfigs.Count > 0)
        {
            ProcessDummyConfigs(xmlTv, dummyVideoStreamConfigs);
        }

        // Process Custom PlayList Configurations
        List<VideoStreamConfig> customVideoStreamConfigs = videoStreamConfigs.Where(a => a.EPGNumber == EPGHelper.CustomPlayListId).ToList();
        if (customVideoStreamConfigs.Count > 0)
        {
            ProcessCustomPlaylists(xmlTv, customVideoStreamConfigs);
        }

        // Process EPG Files
        List<EPGFile> epgFiles = await EPGService.GetEPGFilesAsync();
        if (epgFiles.Count > 0)
        {
            await ProcessEPGFileConfigsAsync(xmlTv, videoStreamConfigs, epgFiles);
        }
    }
    private async Task ProcessScheduleDirectConfigsAsync(XMLTV xmlTv, List<VideoStreamConfig> SDVideoStreamConfigs)
    {
        if (!xmlDict.TryGetValue(EPGHelper.SchedulesDirectId, out XMLTV? xml))
        {
            xml = await fileUtilService.ReadXmlFileAsync(BuildInfo.SDXMLFile).ConfigureAwait(false);
            if (xml == null)
            {
                return;
            }
            _ = xmlDict.TryAdd(EPGHelper.SchedulesDirectId, xml);
        }

        (List<XmltvChannel> newChannels, List<XmltvProgramme> newProgrammes) = ProcessXML(xml, SDVideoStreamConfigs);

        xmlTv.Channels.AddRange(newChannels);
        xmlTv.Programs.AddRange(newProgrammes);
    }

    private void ProcessDummyConfigs(XMLTV xmlTv, List<VideoStreamConfig> dummyConfigs)
    {
        ConcurrentBag<XmltvChannel> channels = [];
        ConcurrentBag<XmltvProgramme> programs = [];

        // Process each config in parallel
        _ = Parallel.ForEach(dummyConfigs, config =>
        {
            if (config.OutputProfile is null)
            {
                return;
            }

            XmltvChannel channel = new()
            {
                Id = config.OutputProfile.Id,
                DisplayNames = [new XmltvText { Text = config.Name }],
                Icons = [new XmltvIcon { Src = config.Logo }]
            };
            channels.Add(channel);

            DateTime startTime = DateTime.UtcNow.Date;
            DateTime stopTime = startTime.AddDays(sdSettingsMonitor.CurrentValue.SDEPGDays);
            int fillerProgramLength = sdSettingsMonitor.CurrentValue.XmltvFillerProgramLength;

            while (startTime < stopTime)
            {
                programs.Add(new XmltvProgramme
                {
                    Start = startTime.ToString("yyyyMMddHHmmss 0000", CultureInfo.InvariantCulture),
                    Stop = startTime.AddHours(fillerProgramLength).ToString("yyyyMMddHHmmss 0000", CultureInfo.InvariantCulture),
                    Channel = config.OutputProfile.Id,
                    Titles = [new XmltvText { Language = "en", Text = config.Name }]
                });

                startTime = startTime.AddHours(fillerProgramLength);
            }
        });

        // Add results back to the shared collections
        xmlTv.Channels.AddRange(channels);
        xmlTv.Programs.AddRange(programs);
    }

    private async Task ProcessEPGFileConfigsAsync(XMLTV xmlTv, List<VideoStreamConfig> configs, List<EPGFile> epgFiles)
    {
        // Process EPG files in parallel
        IEnumerable<Task> tasks = epgFiles.Select(async epgFile =>
        {
            // Find matching configs
            List<VideoStreamConfig> matchingConfigs = configs.Where(a => a.EPGNumber == epgFile.EPGNumber).ToList();
            if (matchingConfigs.Count == 0)
            {
                return; // Skip processing if no matching configs
            }

            // Retrieve or read the XMLTV file
            if (!xmlDict.TryGetValue(epgFile.EPGNumber, out XMLTV? xml))
            {
                xml = await fileUtilService.ReadXmlFileAsync(epgFile).ConfigureAwait(false);
                if (xml == null)
                {
                    return; // Skip processing if the XML file is null
                }

                _ = xmlDict.TryAdd(epgFile.EPGNumber, xml);
            }

            // Process the XML to extract channels and programmes
            (List<XmltvChannel> newChannels, List<XmltvProgramme> newProgrammes) = ProcessXML(xml, matchingConfigs);

            // Synchronize additions to shared collections
            lock (xmlTv.Channels)
            {
                xmlTv.Channels.AddRange(newChannels);
            }

            lock (xmlTv.Programs)
            {
                xmlTv.Programs.AddRange(newProgrammes);
            }
        });

        // Await all tasks to complete
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private void ProcessCustomPlaylists(XMLTV xmlTv, List<VideoStreamConfig> customConfigs)
    {
        ConcurrentBag<XmltvChannel> channels = [];
        ConcurrentBag<XmltvProgramme> programs = [];

        _ = Parallel.ForEach(customConfigs, config =>
        {
            XmltvChannel channel = new()
            {
                Id = config.OutputProfile!.Id,
                DisplayNames = [new XmltvText { Text = config.Name }]
            };

            CustomPlayList? nfo = customPlayListBuilder.GetCustomPlayList(config.Name);

            string? logoFile = customPlayListBuilder.GetCustomPlayListLogoFromFileName(config.Name);
            if (logoFile is not null)
            {
                if (!string.IsNullOrEmpty(logoFile))
                {
                    channel.Icons =
                [
                    new XmltvIcon { Src = logoFile }
                ];
                }
                else if (nfo?.FolderNfo != null && !string.IsNullOrEmpty(nfo.FolderNfo.Thumb?.Text))
                {
                    channel.Icons =
                [
                    new XmltvIcon { Src = logoFile }
                ];
                }
            }

            channels.Add(channel);

            List<XmltvProgramme> newProgrammes = logoService.GetXmltvProgrammeForPeriod(config, SMDT.UtcNow, sdSettingsMonitor.CurrentValue.SDEPGDays, config.BaseUrl);
            foreach (XmltvProgramme programme in newProgrammes)
            {
                programme.Channel = config.OutputProfile!.Id;
                programs.Add(programme);
            }
        });

        // Add results to the shared collections after parallel processing
        xmlTv.Channels.AddRange(channels);
        xmlTv.Programs.AddRange(programs);
    }

    private (List<XmltvChannel> xmltvChannels, List<XmltvProgramme> programs) ProcessXML(XMLTV xml, List<VideoStreamConfig> videoStreamConfigs)
    {
        string baseUrl = GetUrlWithPath();
        // Use ToLookup to allow multiple channels with the same ID.
        ILookup<string, XmltvChannel> channelsById = xml.Channels.ToLookup(a => a.Id);

        // Initialize new lists for channels and programmes to be added.
        List<XmltvChannel> newChannels = [];
        List<XmltvProgramme> newProgrammes = [];

        // Iterate over each StationChannelName
        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {
            // Retrieve XmltvChannel using dictionary lookup
            if (channelsById[videoStreamConfig.EPGId] != null)
            {
                // Iterate over all channels associated with the current ID.               
                foreach (XmltvChannel channel in channelsById[videoStreamConfig.EPGId])
                {
                    channel.Id = videoStreamConfig.OutputProfile!.Id;
                    if (channel.Icons?.Count > 0)
                    {
                        foreach (XmltvIcon icon in channel.Icons)
                        {
                            icon.Src = videoStreamConfig.Logo;// $"{baseUrl}/api/files/pr/{icon.Src.GenerateFNV1aHash()}";
                        }
                    }
                    newChannels.Add(channel);
                }

                // Find all programmes associated with the current channel.
                IEnumerable<XmltvProgramme> programmes = xml.Programs.Where(a => a.Channel == videoStreamConfig.EPGId);
                foreach (XmltvProgramme programme in programmes)
                {
                    // Perform a deep copy of the programme to ensure immutability of the original.
                    XmltvProgramme prog = programme.DeepCopy();
                    prog.Channel = videoStreamConfig.OutputProfile!.Id;

                    if (prog.Icons?.Count > 0)
                    {
                        foreach (XmltvIcon icon in prog.Icons)
                        {
                            icon.Src = $"{baseUrl}/api/files/pr/{icon.Src.GenerateFNV1aHash()}";
                        }
                    }
                    newProgrammes.Add(prog);
                }
            }
        }

        // Add accumulated channels and programmes to the xmlTv in a batch operation.
        return (newChannels, newProgrammes);
    }


}