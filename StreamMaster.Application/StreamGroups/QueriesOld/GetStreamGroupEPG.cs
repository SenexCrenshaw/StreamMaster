using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Requests;

using System.Net;
using System.Xml;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

public class GetStreamGroupEPGValidator : AbstractValidator<GetStreamGroupEPG>
{
    public GetStreamGroupEPGValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

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

        List<SMChannel> smChannels = request.StreamGroupId == 0
            ? await Repository.SMChannel.GetQuery().ToListAsync(cancellationToken: cancellationToken)
                : await Repository.SMChannel.GetSMChannelsFromStreamGroup(request.StreamGroupId);

        List<VideoStreamConfig> videoStreamConfigs = [];

        logger.LogInformation("GetStreamGroupEPGHandler: Handling {Count} smStreams", smChannels.Count);

        foreach (SMChannel? smChannel in smChannels.Where(a => !a.IsHidden))
        {
            videoStreamConfigs.Add(new VideoStreamConfig
            {
                Id = smChannel.Id.ToString(),
                M3UFileId = 0,//smChannel.M3UFileId,
                User_Tvg_name = smChannel.Name,
                Tvg_ID = smChannel.EPGId,
                User_Tvg_ID = smChannel.EPGId,
                User_Tvg_Logo = smChannel.Logo,
                User_Tvg_chno = smChannel.ChannelNumber,
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

            videoStreamConfig.IsDummy = epgHelper.IsDummy(videoStreamConfig.User_Tvg_ID);

            if (videoStreamConfig.IsDummy)
            {
                videoStreamConfig.User_Tvg_ID = $"{EPGHelper.DummyId}-{videoStreamConfig.Id}";

                dummyData.FindOrCreateDummyService(videoStreamConfig.User_Tvg_ID, videoStreamConfig);
            }

            if (epgids.Contains(videoStreamConfig.User_Tvg_ID))
            {
                videoStreamConfig.IsDuplicate = true;
            }
            else
            {
                epgids.Add(videoStreamConfig.User_Tvg_ID);
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