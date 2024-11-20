using System.Globalization;

using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Comparer;
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
    private readonly IFileUtilService fileUtilService;
    private readonly ICustomPlayListBuilder customPlayListBuilder;
    private readonly IOptionsMonitor<SDSettings> sdSettingsMonitor;
    private readonly ILogoService logoService;
    public XMLTVBuilder(
        IOptionsMonitor<SDSettings> sdSettingsMonitor,
        IOptionsMonitor<OutputProfileDict> outputProfileDictMonitor,
        IServiceProvider serviceProvider,
        ILogoService logoService,
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

        _dataPreparationService = new DataPreparationService(
            sdSettingsMonitor,
            outputProfileDictMonitor,
            serviceProvider,
            customPlayListBuilder,
            schedulesDirectDataService
            );

        _channelBuilder = new XmltvChannelBuilder(logoService, schedulesDirectDataService, sdSettingsMonitor);

        // Pass the shared data (_seriesDict and _keywordDict) to XmltvProgramBuilder
        _programBuilder = new XmltvProgramBuilder(
            sdSettingsMonitor,
            logoService,
            _dataPreparationService.SeriesDict,
            _dataPreparationService.KeywordDict);
    }
    public async Task<XMLTV?> CreateXmlTv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs, OutputProfileDto outputProfile)
    {
        try
        {
            _dataPreparationService.Initialize(baseUrl, videoStreamConfigs);

            XMLTV xmlTv = InitializeXmlTv();

            await ProcessServices(xmlTv, outputProfile, videoStreamConfigs);

            SortXmlTvEntries(xmlTv);

            return xmlTv;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to create the XMLTV file. Exception: {Message}", ex.Message);
            return null;
        }
    }

    public XMLTV? CreateSDXmlTv(string baseUrl)
    {
        try
        {
            _dataPreparationService.Initialize(baseUrl, null);
            cacheManager.ClearEPGDataByEPGNumber(EPGHelper.SchedulesDirectId);

            XMLTV xmlTv = ProcessSDServices();

            SortXmlTvEntries(xmlTv);
            fileUtilService.ProcessStationChannelNamesAsync(BuildInfo.SDXMLFile, EPGHelper.SchedulesDirectId);
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

    private async Task ProcessServices(XMLTV xmlTv, OutputProfileDto outputProfile, List<VideoStreamConfig> videoStreamConfigs)
    {
        XMLTV? sdXml = null;
        videoStreamConfigs ??= [];

        //List<EPGFile> epgFiles = _dataPreparationService.GetEpgFiles(videoStreamConfigs);
        string baseUrl = GetUrlWithPath();

        //List<string> ids = videoStreamConfigs.Select(a => a.EPGId).ToList();
        List<StationChannelName> stationChannelNames = [];

        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {
            StationChannelName? match = cacheManager.StationChannelNames
            .SelectMany(kvp => kvp.Value)
            .FirstOrDefault(stationchannel => stationchannel.Id == videoStreamConfig.EPGId || stationchannel.Channel == videoStreamConfig.EPGId);
            if (match != null)
            {
                stationChannelNames.Add(match);
                if (videoStreamConfig.EPGId != match.Id)
                {
                    videoStreamConfig.EPGId = match.Id;
                    int a = 1;
                }
            }
            else
            {
                StationChannelName? newChannel = HandleMissing(videoStreamConfig);
                if (newChannel != null)
                {
                    stationChannelNames.Add(newChannel);
                }
            }
        }

        if (stationChannelNames.Count == 0)
        {
            return;
        }
        Dictionary<string, VideoStreamConfig> videoStreamConfigDictionary = videoStreamConfigs.ToDictionary(a => a.EPGId);

        List<StationChannelName> sdSns = stationChannelNames.Where(a => a.EPGNumber == EPGHelper.SchedulesDirectId).ToList();
        if (sdSns.Count > 0)
        {
            if (sdXml is null && File.Exists(BuildInfo.SDXMLFile))
            {
                sdXml = await fileUtilService.ReadXmlFileAsync(BuildInfo.SDXMLFile);
            }

            if (sdXml is not null)
            {
                (List<XmltvChannel> newChannels, List<XmltvProgramme> newProgrammes) = ProcessXML(sdXml, sdSns, outputProfile, videoStreamConfigDictionary);

                // Add accumulated channels and programmes to the xmlTv in a batch operation.
                xmlTv.Channels.AddRange(newChannels);
                xmlTv.Programs.AddRange(newProgrammes);
            }
        }

        List<StationChannelName> customSNs = stationChannelNames.Where(a => a.EPGNumber == EPGHelper.CustomPlayListId).ToList();
        if (customSNs.Count > 0)
        {
            foreach (StationChannelName customsn in customSNs)
            {
                XmltvChannel channel = new()
                {
                    Id = customsn.Channel,
                    DisplayNames = [new XmltvText { Text = customsn.DisplayName }]
                };

                PlayList.Models.CustomPlayList? nfo = customPlayListBuilder.GetCustomPlayList(customsn.ChannelName);
                string? logoFile = customPlayListBuilder.GetCustomPlayListLogoFromFileName(customsn.ChannelName);

                if (!string.IsNullOrEmpty(logoFile))
                {
                    channel.Icons =
                    [
                        new XmltvIcon {
                            Src=logoService.GetLogoUrl(logoFile, baseUrl),
                        }
                    ];
                }
                else if (nfo?.FolderNfo is not null && nfo.FolderNfo.Fanart != null && !string.IsNullOrEmpty(nfo.FolderNfo.Thumb?.Text))
                {
                    channel.Icons =
                    [
                        new XmltvIcon {
                           Src=logoService.GetLogoUrl(nfo.FolderNfo.Thumb.Text, baseUrl),
                        }
                    ];
                }

                xmlTv.Channels.Add(channel);

                List<XmltvProgramme> newProgrammes = logoService.GetXmltvProgrammeForPeriod(customsn, SMDT.UtcNow, sdSettingsMonitor.CurrentValue.SDEPGDays, baseUrl);
                xmlTv.Programs.AddRange(newProgrammes);
            }
        }

        List<EPGFile> epgFiles = _dataPreparationService.GetEpgFiles(videoStreamConfigs);

        foreach (EPGFile epgFile in epgFiles)
        {
            List<StationChannelName> sns = stationChannelNames.Where(a => a.EPGNumber == epgFile.EPGNumber).ToList();
            if (sns.Count == 0)
            {
                continue;
            }

            // Read the XML file asynchronously
            XMLTV? xml = await fileUtilService.ReadXmlFileAsync(epgFile);

            if (xml is not null)
            {
                (List<XmltvChannel> newChannels, List<XmltvProgramme> newProgrammes) = ProcessXML(xml, sns, outputProfile, videoStreamConfigDictionary);

                // Add accumulated channels and programmes to the xmlTv in a batch operation.
                xmlTv.Channels.AddRange(newChannels);

                xmlTv.Programs.AddRange(newProgrammes);
            }
        }

        //foreach (StationChannelName service in matchingStationChannelNames)
        //{
        //    VideoStreamConfig? videoStreamConfig = videoStreamConfigs.Find(a => service.Id == a.EPGId);

        //    XmltvChannel channel = _channelBuilder.BuildXmltvChannel(service, videoStreamConfig, outputProfile, _dataPreparationService.BaseUrl);

        //    xmlTv.Channels.Add(channel);

        //    _dataPreparationService.AdjustServiceSchedules(service);

        //    List<XmltvProgramme> xmltvProgrammes = GetPrograms(service, videoStreamConfigs, baseUrl, channel.Id, epgFiles);

        //    xmlTv.Programs.AddRange(xmltvProgrammes);
        //}
    }

    private static StationChannelName? HandleMissing(VideoStreamConfig videoStreamConfig)
    {
        if (videoStreamConfig.EPGId.StartsWith(EPGHelper.CustomPlayListId.ToString() + "-"))
        {
            string epgId = videoStreamConfig.EPGId;
            if (EPGHelper.IsValidEPGId(videoStreamConfig.EPGId))
            {
                (_, epgId) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
            }
            return new StationChannelName(videoStreamConfig.ChannelNumber.ToString(), videoStreamConfig.Name, videoStreamConfig.Name, EPGHelper.CustomPlayListId);

        }
        else if (!videoStreamConfig.EPGId.StartsWith(EPGHelper.DummyId.ToString()) && !videoStreamConfig.EPGId.StartsWith(EPGHelper.SchedulesDirectId.ToString()))
        {
            string epgId = videoStreamConfig.EPGId;
            if (EPGHelper.IsValidEPGId(videoStreamConfig.EPGId))
            {
                (_, epgId) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
            }
            return new StationChannelName(epgId, videoStreamConfig.Name, videoStreamConfig.Name, EPGHelper.DummyId);

        }
        if (EPGHelper.IsValidEPGId(videoStreamConfig.EPGId))
        {
            string epgId = videoStreamConfig.EPGId;
            int epgNumber = 0;
            (epgNumber, epgId) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
            return new StationChannelName(videoStreamConfig.EPGId, videoStreamConfig.Name, videoStreamConfig.Name, epgNumber);
        }
        return new StationChannelName(videoStreamConfig.EPGId, videoStreamConfig.Name, videoStreamConfig.Name, 0);

    }

    private (List<XmltvChannel> xmltvChannels, List<XmltvProgramme> programs) ProcessXML(XMLTV xml, List<StationChannelName> sns, OutputProfileDto outputProfile, Dictionary<string, VideoStreamConfig> videoStreamConfigDictionary)
    {
        string baseUrl = GetUrlWithPath();
        // Use ToLookup to allow multiple channels with the same ID.
        ILookup<string, XmltvChannel>? channelLookup = xml.Channels.ToLookup(a => a.Id);

        // Using HashSet for quick membership testing for channel names.
        HashSet<string> snsIds = sns.Select(a => a.Channel).ToHashSet();

        // Initialize new lists for channels and programmes to be added.
        List<XmltvChannel> newChannels = [];
        List<XmltvProgramme> newProgrammes = [];

        // Iterate over each StationChannelName
        foreach (StationChannelName sn in sns)
        {
            // Retrieve videoStreamConfig using dictionary lookup
            if (videoStreamConfigDictionary.TryGetValue(sn.Id, out VideoStreamConfig? videoStreamConfig)
                && channelLookup?[sn.Channel] != null)
            {
                // Iterate over all channels associated with the current ID.
                foreach (XmltvChannel channel in channelLookup[sn.Channel])
                {
                    // Build the XMLTV channel using the existing channel and video stream config.
                    XmltvChannel channel2 = _channelBuilder.BuildXmltvChannel(channel, videoStreamConfig, outputProfile, _dataPreparationService.BaseUrl);
                    newChannels.Add(channel2);
                }

                // Find all programmes associated with the current channel.
                IEnumerable<XmltvProgramme> programmes = xml.Programs.Where(a => a.Channel == sn.Channel);
                foreach (XmltvProgramme programme in programmes)
                {
                    // Perform a deep copy of the programme to ensure immutability of the original.
                    XmltvProgramme prog = programme.DeepCopy();
                    prog.Channel = XmltvChannelBuilder.GetChannelId(videoStreamConfig, outputProfile);
                    if (prog.Icons?.Count > 0)
                    {
                        foreach (XmltvIcon icon in prog.Icons)
                        {
                            icon.Src = logoService.GetLogoUrl(icon.Src, baseUrl);
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

        Parallel.ForEach(services, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, service =>
        {
            XmltvChannel channel = _channelBuilder.BuildXmltvChannel(service, _dataPreparationService.BaseUrl, true);
            List<MxfScheduleEntry> scheduleEntries = service.MxfScheduleEntries.ScheduleEntry;

            lock (xmlTv.Channels)
            {
                xmlTv.Channels.Add(channel);
            }

            //_dataPreparationService.AdjustServiceSchedules(service);

            List<XmltvProgramme> xmltvProgrammes = scheduleEntries.AsParallel().Select(scheduleEntry =>
                    _programBuilder.BuildXmltvProgram(scheduleEntry, channel.Id)).ToList();

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
