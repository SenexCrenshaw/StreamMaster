using FluentValidation;

using Microsoft.AspNetCore.Http;

using System.Net;
using System.Xml;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupEPG(int StreamGroupId, int StreamGroupProfileId) : IRequest<string>;
public class GetStreamGroupEPGHandler(IHttpContextAccessor httpContextAccessor, IEPGHelper epgHelper, IXMLTVBuilder xMLTVBuilder, ILogger<GetStreamGroupEPG> logger, ISchedulesDirectDataService schedulesDirectDataService, IRepositoryWrapper Repository, IOptionsMonitor<Setting> intsettings)
    : IRequestHandler<GetStreamGroupEPG, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly Setting settings = intsettings.CurrentValue;


    //private readonly ParallelOptions parallelOptions = new()
    //{
    //    MaxDegreeOfParallelism = Environment.ProcessorCount
    //};

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupEPG request, CancellationToken cancellationToken)
    {

        List<SMChannel> smChannels = await Repository.SMChannel.GetSMChannelsFromStreamGroup(request.StreamGroupId);

        if (!smChannels.Any())
        {
            return "";
        }

        List<VideoStreamConfig> videoStreamConfigs = [];

        logger.LogInformation("GetStreamGroupEPGHandler: Handling {Count} smStreams", smChannels.Count);

        foreach (SMChannel? smChannel in smChannels.Where(a => !a.IsHidden))
        {
            videoStreamConfigs.Add(new VideoStreamConfig
            {
                Id = smChannel.Id,
                Name = smChannel.Name,
                EPGId = smChannel.EPGId,
                Logo = smChannel.Logo,
                ChannelNumber = smChannel.ChannelNumber,
                TimeShift = smChannel.TimeShift,
                IsDuplicate = false,
                IsDummy = false
            });
        }


        ConcurrentHashSet<string> epgids = [];

        ISchedulesDirectData dummyData = schedulesDirectDataService.DummyData();

        List<MxfService> allservices = schedulesDirectDataService.AllServices;

        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {

            videoStreamConfig.IsDummy = epgHelper.IsDummy(videoStreamConfig.EPGId);

            if (videoStreamConfig.IsDummy)
            {
                videoStreamConfig.EPGId = $"{EPGHelper.DummyId}-{videoStreamConfig.Id}";

                dummyData.FindOrCreateDummyService(videoStreamConfig.EPGId, videoStreamConfig);
            }

            if (epgids.Contains(videoStreamConfig.EPGId))
            {
                videoStreamConfig.IsDuplicate = true;
            }
            else
            {
                epgids.Add(videoStreamConfig.EPGId);
            }
        }

        XMLTV epgData = xMLTVBuilder.CreateXmlTv(_httpContextAccessor.GetUrl(), videoStreamConfigs) ?? new XMLTV();

        return SerializeXMLTVData(epgData);
    }

    private string SerializeXMLTVData(XMLTV xmltv)
    {

        XmlSerializerNamespaces ns = new();
        ns.Add("", "");

        // Create a Utf8StringWriter
        using Utf8StringWriter textWriter = new();

        XmlWriterSettings xmlSettings = new()
        {
            Indent = settings.PrettyEPG,
            OmitXmlDeclaration = true,
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
    private string GetApiUrl(SMFileTypes path, string source)
    {
        string url = _httpContextAccessor.GetUrl();
        return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }
}