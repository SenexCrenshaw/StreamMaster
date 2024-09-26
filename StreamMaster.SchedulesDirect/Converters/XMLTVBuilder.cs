using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Comparer;
using StreamMaster.Domain.Models;
using StreamMaster.PlayList;

using System.Globalization;

namespace StreamMaster.SchedulesDirect.Converters;

public class XMLTVBuilder : IXMLTVBuilder
{
    private readonly DataPreparationService _dataPreparationService;
    private readonly XmltvChannelBuilder _channelBuilder;
    private readonly XmltvProgramBuilder _programBuilder;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<XMLTVBuilder> logger;

    public XMLTVBuilder(
        IOptionsMonitor<SDSettings> sdSettingsMonitor,
        IOptionsMonitor<OutputProfileDict> outputProfileDictMonitor,
        IServiceProvider serviceProvider,
        ILogoService logoService,
        ICustomPlayListBuilder customPlayListBuilder,
        ISchedulesDirectDataService schedulesDirectDataService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<XMLTVBuilder> logger)
    {
        this.logger = logger;
        this.httpContextAccessor = httpContextAccessor;

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
    public XMLTV? CreateXmlTv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs, OutputProfileDto outputProfile)
    {
        try
        {
            _dataPreparationService.Initialize(baseUrl, videoStreamConfigs);

            List<MxfService> servicesToProcess = _dataPreparationService.GetServicesToProcess(videoStreamConfigs);

            XMLTV xmlTv = InitializeXmlTv();

            ProcessServices(servicesToProcess, xmlTv, outputProfile, videoStreamConfigs);

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

            List<MxfService> services = _dataPreparationService.GetAllSdServices();

            XMLTV xmlTv = InitializeXmlTv();

            OutputProfileDto outputProfile = _dataPreparationService.GetDefaultOutputProfile();

            ProcessServices(services, xmlTv, outputProfile);

            SortXmlTvEntries(xmlTv);

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
    private void ProcessServices(List<MxfService> services, XMLTV xmlTv, OutputProfileDto outputProfile, List<VideoStreamConfig>? videoStreamConfigs = null)
    {
        videoStreamConfigs ??= [];

        List<EPGFile> epgFiles = _dataPreparationService.GetEpgFiles(videoStreamConfigs);
        string baseUrl = GetUrlWithPath();

        Parallel.ForEach(services, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, service =>
        {
            VideoStreamConfig? videoStreamConfig = videoStreamConfigs.Find(a => service.StationId == a.EPGId);

            XmltvChannel channel = _channelBuilder.BuildXmltvChannel(service, videoStreamConfig, outputProfile, _dataPreparationService.BaseUrl);

            lock (xmlTv.Channels)
            {
                xmlTv.Channels.Add(channel);
            }

            _dataPreparationService.AdjustServiceSchedules(service);

            List<XmltvProgramme> xmltvProgrammes = GetPrograms(service, videoStreamConfigs, baseUrl, channel.Id, epgFiles);

            lock (xmlTv.Programs)
            {
                xmlTv.Programs.AddRange(xmltvProgrammes);
            }
        });
    }

    private List<XmltvProgramme> GetPrograms(MxfService mxfService, List<VideoStreamConfig> videoStreamConfigs, string baseUrl, string ChannelId, List<EPGFile> epgFiles)
    {
        VideoStreamConfig? videoStreamConfig = videoStreamConfigs.Find(a => mxfService.StationId == a.EPGId);
        return videoStreamConfig == null ? ([]) : GetPrograms(mxfService, baseUrl, ChannelId, videoStreamConfig, epgFiles);
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
