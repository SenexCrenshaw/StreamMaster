using System.Collections.Concurrent;
using System.Globalization;

using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Comparer;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;
using StreamMaster.PlayList;
using StreamMaster.Streams.Domain.Interfaces;

namespace StreamMaster.SchedulesDirect.Converters;

public class XMLTVBuilder : IXMLTVBuilder
{
    private readonly DataPreparationService _dataPreparationService;
    private readonly XmltvChannelBuilder _channelBuilder;
    private readonly XmltvProgramBuilder _programBuilder;
    private readonly ILogger<XMLTVBuilder> logger;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ICacheManager cacheManager;
    private readonly IProfileService profileService;
    private readonly IFileUtilService fileUtilService;
    private readonly ICustomPlayListBuilder customPlayListBuilder;
    private readonly IOptionsMonitor<SDSettings> sdSettingsMonitor;
    private readonly IOptionsMonitor<Setting> settings;
    private readonly ILogoService logoService;

    private readonly ConcurrentDictionary<int, XMLTV> xmlDict = new();

    public XMLTVBuilder(
        IOptionsMonitor<SDSettings> sdSettingsMonitor,
             IOptionsMonitor<Setting> settings,
        IOptionsMonitor<OutputProfileDict> outputProfileDictMonitor,
        IServiceProvider serviceProvider,
        ILogoService logoService,
        IProfileService ProfileService,
        IFileUtilService fileUtilService,
        ICacheManager cacheManager,
        ICustomPlayListBuilder customPlayListBuilder,
        ISchedulesDirectDataService schedulesDirectDataService,
        IHttpContextAccessor httpContextAccessor,

        ILogger<XMLTVBuilder> logger)
    {
        this.logoService = logoService;
        this.customPlayListBuilder = customPlayListBuilder;
        this.fileUtilService = fileUtilService;
        this.cacheManager = cacheManager;
        this.logger = logger;
        this.httpContextAccessor = httpContextAccessor;
        this.sdSettingsMonitor = sdSettingsMonitor;
        this.settings = settings;
        profileService = ProfileService;
        _dataPreparationService = new DataPreparationService(
            sdSettingsMonitor,
            outputProfileDictMonitor,
            serviceProvider,
            customPlayListBuilder,
            schedulesDirectDataService
            );

        _channelBuilder = new XmltvChannelBuilder(logoService, schedulesDirectDataService, sdSettingsMonitor);

        // Pass the shared data (_seriesDict and _keywordDict) to XmltvProgramBuilder
        _programBuilder = new XmltvProgramBuilder(settings, sdSettingsMonitor, logoService);
    }
    public async Task<XMLTV?> CreateXmlTv(List<VideoStreamConfig> videoStreamConfigs)
    {
        xmlDict.Clear();
        try
        {
            //_dataPreparationService.Initialize(baseUrl, videoStreamConfigs);

            XMLTV xmlTv = InitializeXmlTv();

            await ProcessServicesAsync(xmlTv, videoStreamConfigs);

            SortXmlTvEntries(xmlTv);

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

    public XMLTV? CreateSDXmlTv(string baseUrl)
    {
        try
        {
            //_dataPreparationService.Initialize(baseUrl, null);
            cacheManager.ClearEPGDataByEPGNumber(EPGHelper.SchedulesDirectId);

            XMLTV xmlTv = ProcessSDServices();

            SortXmlTvEntries(xmlTv);
            _ = fileUtilService.ProcessStationChannelNamesAsync(BuildInfo.SDXMLFile, EPGHelper.SchedulesDirectId);
            return xmlTv;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to create the XMLTV file. Exception: {Message}", ex.Message);
            return null;
        }
    }

    private static XMLTV InitializeXmlTv()
    {
        return new XMLTV
        {
            Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            SourceInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
            SourceInfoName = "Stream Master",
            GeneratorInfoName = "Stream Master",
            GeneratorInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
            Channels = [],
            Programs = []
        };
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
        List<EPGFile> epgFiles = _dataPreparationService.GetEpgFiles(videoStreamConfigs);
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
                DisplayNames = [new XmltvText { Text = config.Name }]
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

            PlayList.Models.CustomPlayList? nfo = customPlayListBuilder.GetCustomPlayList(config.Name);
            string? logoFile = customPlayListBuilder.GetCustomPlayListLogoFromFileName(config.Name);

            if (!string.IsNullOrEmpty(logoFile))
            {
                channel.Icons =
            [
                new XmltvIcon { Src = logoService.GetLogoUrl(logoFile, config.BaseUrl, SMStreamTypeEnum.CustomPlayList) }
            ];
            }
            else if (nfo?.FolderNfo != null && !string.IsNullOrEmpty(nfo.FolderNfo.Thumb?.Text))
            {
                channel.Icons =
            [
                new XmltvIcon { Src = logoService.GetLogoUrl(nfo.FolderNfo.Thumb.Text, config.BaseUrl,SMStreamTypeEnum.CustomPlayList) }
            ];
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

        // Using HashSet for quick membership testing for channel names.
        // HashSet<string> snsIds = videoStreamConfigs.Select(a => a.OutputProfile.Id).ToHashSet();

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
                //foreach (XmltvChannel channel in channelsById[videoStreamConfig.OutputProfile.Id])
                //{
                //    XmltvChannel channel2 = _channelBuilder.BuildXmltvChannel(channel, videoStreamConfig);
                //    newChannels.Add(channel2);
                //}
                //newChannels.AddRange();
                foreach (XmltvChannel channel in channelsById[videoStreamConfig.EPGId])
                {
                    channel.Id = videoStreamConfig.OutputProfile!.Id;
                    newChannels.Add(channel);
                }

                // Find all programmes associated with the current channel.
                IEnumerable<XmltvProgramme> programmes = xml.Programs.Where(a => a.Channel == videoStreamConfig.EPGId);
                foreach (XmltvProgramme programme in programmes)
                {
                    // Perform a deep copy of the programme to ensure immutability of the original.
                    XmltvProgramme prog = programme.DeepCopy();
                    prog.Channel = videoStreamConfig.OutputProfile!.Id;
                    //prog.Channel = channel.Id;
                    if (prog.Icons?.Count > 0)
                    {
                        foreach (XmltvIcon icon in prog.Icons)
                        {
                            icon.Src = logoService.GetLogoUrl(icon.Src, baseUrl, SMStreamTypeEnum.CustomPlayList);
                        }

                    }
                    newProgrammes.Add(prog);
                }
            }
        }

        // Add accumulated channels and programmes to the xmlTv in a batch operation.
        return (newChannels, newProgrammes);
    }

    private XMLTV ProcessSDServices()
    {
        List<MxfService> services = _dataPreparationService.GetAllSdServices();

        XMLTV xmlTv = InitializeXmlTv();

        string baseUrl = GetUrlWithPath();

        _ = Parallel.ForEach(services, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, service =>
        {
            XmltvChannel channel = _channelBuilder.BuildXmltvChannel(service, _dataPreparationService.BaseUrl, true);
            List<MxfScheduleEntry> scheduleEntries = service.MxfScheduleEntries.ScheduleEntry;

            lock (xmlTv.Channels)
            {
                xmlTv.Channels.Add(channel);
            }

            _dataPreparationService.AdjustServiceSchedules(service);

            List<XmltvProgramme> xmltvProgrammes = scheduleEntries.AsParallel().Select(scheduleEntry =>
                    _programBuilder.BuildXmltvProgram(scheduleEntry, channel.Id, 0, baseUrl)).ToList();

            lock (xmlTv.Programs)
            {
                xmlTv.Programs.AddRange(xmltvProgrammes);
            }
        });

        return xmlTv;
    }

    public List<XmltvProgramme> GetPrograms(MxfService mxfService, string baseUrl, string ChannelId, VideoStreamConfig? videoStreamConfig, List<EPGFile>? epgFiles)
    {
        List<MxfScheduleEntry> scheduleEntries = mxfService.MxfScheduleEntries.ScheduleEntry;
        int timeShift = (epgFiles == null || videoStreamConfig == null) ? 0 : _dataPreparationService.GetTimeShift(videoStreamConfig, epgFiles);

        List<XmltvProgramme> xmltvProgrammes = scheduleEntries.AsParallel().Select(scheduleEntry =>
            _programBuilder.BuildXmltvProgram(scheduleEntry, ChannelId, timeShift, baseUrl)).ToList();

        return xmltvProgrammes;
    }

    private static void SortXmlTvEntries(XMLTV xmlTv)
    {
        xmlTv.Channels = [.. xmlTv.Channels.OrderBy(c => c.Id, new NumericStringComparer())];
        xmlTv.Programs = [.. xmlTv.Programs.OrderBy(p => p.Channel).ThenBy(p => p.StartDateTime)];
    }
}
