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
    //private readonly ConcurrentDictionary<int, XMLTV> xmlDict = new();

    public async Task<XMLTV?> CreateXmlTv(List<VideoStreamConfig> videoStreamConfigs, CancellationToken cancellationToken)
    {
        //xmlDict.Clear();
        try
        {
            XMLTV xmlTv = XMLUtil.NewXMLTV;

            await ProcessServicesAsync(xmlTv, videoStreamConfigs, cancellationToken);

            xmlTv.SortXmlTv();
            //xmlDict.Clear();
            return xmlTv;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to create the XMLTV file. Exception: {Message}", ex.Message);
            return null;
        }
        finally
        {
            //xmlDict.Clear();
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

    private async Task ProcessServicesAsync(XMLTV xmlTv, List<VideoStreamConfig> videoStreamConfigs, CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();
        if (videoStreamConfigs == null || videoStreamConfigs.Count == 0)
        {
            return;
        }

        // Dictionary for quick lookup by EPGId
        //Dictionary<string, VideoStreamConfig> videoStreamConfigDictionary = videoStreamConfigs.ToDictionary(a => a.EPGId);

        // Process Schedules Direct Configurations
        List<VideoStreamConfig> sdVideoStreamConfigs = [.. videoStreamConfigs.Where(a => a.EPGNumber == EPGHelper.SchedulesDirectId)];
        if (sdVideoStreamConfigs.Count > 0)
        {
            await ProcessScheduleDirectConfigsAsync(xmlTv, sdVideoStreamConfigs);
        }
        cancellation.ThrowIfCancellationRequested();
        // Process Dummy Configurations
        List<VideoStreamConfig> dummyVideoStreamConfigs = [.. videoStreamConfigs.Where(a => a.EPGNumber == EPGHelper.DummyId)];
        if (dummyVideoStreamConfigs.Count > 0)
        {
            ProcessDummyConfigs(xmlTv, dummyVideoStreamConfigs);
        }
        cancellation.ThrowIfCancellationRequested();
        // Process Custom PlayList Configurations
        List<VideoStreamConfig> customVideoStreamConfigs = [.. videoStreamConfigs.Where(a => a.EPGNumber == EPGHelper.CustomPlayListId)];
        if (customVideoStreamConfigs.Count > 0)
        {
            ProcessCustomPlaylists(xmlTv, customVideoStreamConfigs);
        }
        cancellation.ThrowIfCancellationRequested();
        // Process EPG Files
        List<EPGFile> epgFiles = await EPGService.GetEPGFilesAsync();
        if (epgFiles.Count > 0)
        {
            cancellation.ThrowIfCancellationRequested();
            await ProcessEPGFileConfigsAsync(xmlTv, videoStreamConfigs, epgFiles, cancellation);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private async Task ProcessScheduleDirectConfigsAsync(XMLTV xmlTv, List<VideoStreamConfig> SDVideoStreamConfigs)
    {
        //if (!xmlDict.TryGetValue(EPGHelper.SchedulesDirectId, out XMLTV? xml))
        //{
        //    xml = await fileUtilService.ReadXmlFileAsync(BuildInfo.SDXMLFile).ConfigureAwait(false);
        //    if (xml == null)
        //    {
        //        return;
        //    }
        //    _ = xmlDict.TryAdd(EPGHelper.SchedulesDirectId, xml);
        //}

        XMLTV? xml = await fileUtilService.ReadXmlFileAsync(BuildInfo.SDXMLFile).ConfigureAwait(false);
        if (xml == null)
        {
            return;
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

    private async Task ProcessEPGFileConfigsAsync(XMLTV xmlTv, List<VideoStreamConfig> configs, List<EPGFile> epgFiles, CancellationToken cancellationToken)
    {
        foreach (EPGFile epgFile in epgFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Find matching configs
            List<VideoStreamConfig> matchingConfigs = [.. configs.Where(a => a.EPGNumber == epgFile.EPGNumber)];
            if (matchingConfigs.Count == 0)
            {
                continue; // Skip processing if no matching configs
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Retrieve or read the XMLTV file
            XMLTV? xml = await fileUtilService.ReadXmlFileAsync(epgFile).ConfigureAwait(false);
            if (xml == null)
            {
                continue; // Skip processing if the XML file is null
            }

            cancellationToken.ThrowIfCancellationRequested();

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
        }
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

        // Precompute lookup dictionaries for channels and programs
        Dictionary<string, List<XmltvChannel>> channelsById = xml.Channels
            .GroupBy(channel => channel.Id)
            .ToDictionary(group => group.Key, group => group.ToList());

        Dictionary<string, List<XmltvProgramme>> programsByChannel = xml.Programs
            .GroupBy(program => program.Channel)
            .ToDictionary(group => group.Key, group => group.ToList());

        // Initialize new lists for channels and programmes to be added
        List<XmltvChannel> newChannels = [];
        List<XmltvProgramme> newProgrammes = [];

        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {
            if (channelsById.TryGetValue(videoStreamConfig.EPGId, out List<XmltvChannel>? matchingChannels))
            {
                XmltvChannel xmlChannel = matchingChannels[0];

                XmltvChannel updatedChannel = new()
                {
                    Id = videoStreamConfig.OutputProfile!.Id,
                    DisplayNames = xmlChannel.DisplayNames, // Reuse immutable properties
                    Icons = xmlChannel.Icons?.Select(_ => new XmltvIcon
                    {
                        Src = videoStreamConfig.Logo // Update logo with the provided one
                    }).ToList()
                };
                newChannels.Add(updatedChannel);
            }

            if (programsByChannel.TryGetValue(videoStreamConfig.EPGId, out List<XmltvProgramme>? matchingPrograms))
            {
                foreach (XmltvProgramme? program in matchingPrograms)
                {
                    XmltvProgramme newProgram = program.DeepCopy();

                    newProgram.Channel = videoStreamConfig.OutputProfile!.Id;
                    newProgram.Icons = program.Icons?.Select(icon => new XmltvIcon
                    {
                        Src = $"{baseUrl}/api/files/pr/{icon.Src.GenerateFNV1aHash()}"
                    }).ToList();

                    newProgrammes.Add(newProgram);
                }
            }
        }

        return (newChannels, newProgrammes);
    }
}