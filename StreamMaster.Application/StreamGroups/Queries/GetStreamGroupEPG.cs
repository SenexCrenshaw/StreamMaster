using Microsoft.AspNetCore.Http;

using System.Xml;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.Queries;

public class ChannelNumberConfig
{
    public int SMChannelId { get; set; }
    public int ChannelNumber
    {
        get; set;
    }
    public int M3UFileId { get; set; }
    public int FilePosition { get; set; }
}

[RequireAll]
public record GetStreamGroupEPG(int StreamGroupId, int StreamGroupProfileId) : IRequest<string>;
public class GetStreamGroupEPGHandler(IHttpContextAccessor httpContextAccessor, ISender sender, IEPGHelper epgHelper, IXMLTVBuilder xMLTVBuilder, ILogger<GetStreamGroupEPG> logger, ISchedulesDirectDataService schedulesDirectDataService, IOptionsMonitor<Setting> intsettings)
    : IRequestHandler<GetStreamGroupEPG, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    //private readonly ParallelOptions parallelOptions = new()
    //{
    //    MaxDegreeOfParallelism = Environment.ProcessorCount
    //};
    //private readonly ConcurrentDictionary<int, VideoStreamConfig> existingNumbers = new();
    //private readonly ConcurrentHashSet<int> usedNumbers = [];
    //private readonly int currentChannelNumber;

    //private int GetNextChannelNumber(int channelNumber, bool ignoreExisting)
    //{
    //    if (ignoreExisting)
    //    {
    //        return getNext();
    //    }

    //    if (existingNumbers.ContainsKey(channelNumber))
    //    {
    //        if (usedNumbers.Add(channelNumber))
    //        {
    //            return channelNumber;
    //        }
    //    }

    //    return getNext();
    //}

    //private int getNext()
    //{
    //    ++currentChannelNumber;
    //    while (!usedNumbers.Add(currentChannelNumber))
    //    {
    //        ++currentChannelNumber;
    //    }
    //    return currentChannelNumber;
    //}

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupEPG request, CancellationToken cancellationToken)
    {

        (List<VideoStreamConfig>? videoStreamConfigs, OutputProfile? profile) = await sender.Send(new GetStreamGroupVideoConfigs(request.StreamGroupId, request.StreamGroupProfileId), cancellationToken);

        if (videoStreamConfigs is null || profile is null)
        {
            return string.Empty;
        }
        logger.LogInformation("GetStreamGroupEPGHandler: Handling {Count} channels", videoStreamConfigs.Count);


        ConcurrentHashSet<string> epgids = [];

        ISchedulesDirectData dummyData = schedulesDirectDataService.DummyData();

        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {

            videoStreamConfig.IsDummy = epgHelper.IsDummy(videoStreamConfig.EPGId);

            if (videoStreamConfig.IsDummy)
            {
                videoStreamConfig.EPGId = $"{EPGHelper.DummyId}-{videoStreamConfig.Id}";

                dummyData.FindOrCreateDummyService(videoStreamConfig.EPGId, videoStreamConfig);
            }

            if (!epgids.Add(videoStreamConfig.EPGId))
            {
                videoStreamConfig.IsDuplicate = true;
            }

        }

        XMLTV epgData = xMLTVBuilder.CreateXmlTv(_httpContextAccessor.GetUrl(), videoStreamConfigs, profile) ?? new XMLTV();

        return SerializeXMLTVData(epgData);
    }

    private string SerializeXMLTVData(XMLTV xmltv)
    {
        Setting settings = intsettings.CurrentValue;


        XmlSerializerNamespaces ns = new();
        ns.Add("", "");

        // Create a Utf8StringWriter
        using Utf8StringWriter textWriter = new();

        XmlWriterSettings xmlSettings = new()
        {
            Indent = settings.PrettyEPG,
            //OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.None,
            //NewLineChars = "\n"
        };

        // Create an XmlWriter using Utf8StringWriter
        using XmlWriter writer = XmlWriter.Create(textWriter, xmlSettings);

        XmlSerializer xml = new(typeof(XMLTV));

        // Serialize XML data to the Utf8StringWriter
        xml.Serialize(writer, xmltv, ns);

        // Get the XML string from the Utf8StringWriter
        string xmlText = textWriter.ToString();

        return xmlText;
    }
    //private string GetApiUrl(SMFileTypes path, string source)
    //{
    //    string url = _httpContextAccessor.GetUrl();
    //    return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    //}
}