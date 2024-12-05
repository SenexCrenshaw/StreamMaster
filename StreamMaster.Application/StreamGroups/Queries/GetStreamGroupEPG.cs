using System.Xml;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupEPG(int StreamGroupProfileId) : IRequest<string>;
public class GetStreamGroupEPGHandler(IStreamGroupService streamGroupService, IXMLTVBuilder xMLTVBuilder, IOptionsMonitor<Setting> intSettings)
    : IRequestHandler<GetStreamGroupEPG, string>
{
    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupEPG request, CancellationToken cancellationToken)
    {
        (List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile) = await streamGroupService.GetStreamGroupVideoConfigsAsync(request.StreamGroupProfileId);

        if (videoStreamConfigs is null || streamGroupProfile is null)
        {
            return string.Empty;
        }

        HashSet<string> epgIds = [];

        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {
            videoStreamConfig.IsDuplicate = !epgIds.Add(videoStreamConfig.EPGId);
        }

        XMLTV epgData = await xMLTVBuilder.CreateXmlTv(videoStreamConfigs,cancellationToken) ?? new XMLTV();

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
}