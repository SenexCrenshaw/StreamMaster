using System.Xml;
using System.Xml.Serialization;

using Microsoft.AspNetCore.Http;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupEPG(int StreamGroupProfileId) : IRequest<string>;
public class GetStreamGroupEPGHandler(IHttpContextAccessor httpContextAccessor, IProfileService profileService, IStreamGroupService streamGroupService, IEPGHelper epgHelper, IXMLTVBuilder xMLTVBuilder, ISchedulesDirectDataService schedulesDirectDataService, IOptionsMonitor<Setting> intSettings)
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

        ConcurrentHashSet<string> epgids = [];

        ISchedulesDirectData dummyData = schedulesDirectDataService.DummyData();

        //MATCH
        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {
            videoStreamConfig.IsDummy = epgHelper.IsDummy(videoStreamConfig.EPGId);

            if (videoStreamConfig.IsDummy)
            {
                videoStreamConfig.EPGId = $"{EPGHelper.DummyId}-{videoStreamConfig.Id}";

                await dummyData.FindOrCreateDummyService(videoStreamConfig.EPGId, videoStreamConfig);
            }

            if (videoStreamConfig.IsCustom)
            {
                //  videoStreamConfig.EPGId = $"{EPGHelper.CustomPlayListId}-{videoStreamConfig.Id}";
            }

            if (videoStreamConfig.IsIntro)
            {
                // videoStreamConfig.EPGId = $"{EPGHelper.IntroPlayListId}-{videoStreamConfig.Id}";
            }

            if (!epgids.Add(videoStreamConfig.EPGId))
            {
                videoStreamConfig.IsDuplicate = true;
            }
        }
        OutputProfileDto outputProfile = profileService.GetOutputProfile(streamGroupProfile.OutputProfileName);

        XMLTV epgData = await xMLTVBuilder.CreateXmlTv(httpContextAccessor.GetUrl(), videoStreamConfigs, outputProfile) ?? new XMLTV();

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