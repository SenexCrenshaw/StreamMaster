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
public class GetStreamGroupEPGHandler(IHttpContextAccessor httpContextAccessor, IProfileService profileService, IStreamGroupService streamGroupService, ISender sender, IEPGHelper epgHelper, IXMLTVBuilder xMLTVBuilder, ILogger<GetStreamGroupEPG> logger, ISchedulesDirectDataService schedulesDirectDataService, IOptionsMonitor<Setting> intSettings)
    : IRequestHandler<GetStreamGroupEPG, string>
{

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupEPG request, CancellationToken cancellationToken)
    {

        (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await streamGroupService.GetStreamGroupVideoConfigs(request.StreamGroupId, request.StreamGroupProfileId);

        if (videoStreamConfigs is null || streamGroupProfile is null)
        {
            return string.Empty;
        }

        ConcurrentHashSet<string> epgids = [];

        ISchedulesDirectData dummyData = schedulesDirectDataService.DummyData();

        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {

            videoStreamConfig.IsDummy = epgHelper.IsDummy(videoStreamConfig.EPGId);

            if (videoStreamConfig.IsDummy)
            {
                videoStreamConfig.EPGId = $"{EPGHelper.DummyId}-{videoStreamConfig.Id}";

                _ = dummyData.FindOrCreateDummyService(videoStreamConfig.EPGId, videoStreamConfig);
            }

            if (!epgids.Add(videoStreamConfig.EPGId))
            {
                videoStreamConfig.IsDuplicate = true;
            }

        }
        OutputProfileDto outputProfile = profileService.GetOutputProfile(streamGroupProfile.OutputProfileName);


        XMLTV epgData = xMLTVBuilder.CreateXmlTv(httpContextAccessor.GetUrl(), videoStreamConfigs, outputProfile) ?? new XMLTV();

        return SerializeXMLTVData(epgData);
    }

    private string SerializeXMLTVData(XMLTV xmltv)
    {
        Setting settings = intSettings.CurrentValue;


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