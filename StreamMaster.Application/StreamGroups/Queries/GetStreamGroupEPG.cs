using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Requests;
using StreamMaster.SchedulesDirect.Domain.Enums;

using System.Net;
using System.Xml;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.Queries;

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

        List<VideoStreamDto> videoStreams = [];

        videoStreams = request.StreamGroupId == 0
            ? await Repository.VideoStream.GetVideoStreams()
            : await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.StreamGroupId, cancellationToken);

        List<VideoStreamConfig> videoStreamConfigs = [];

        logger.LogInformation("GetStreamGroupEPGHandler: Handling {Count} videoStreams", videoStreams.Count);

        foreach (VideoStreamDto? videoStream in videoStreams.Where(a => !a.IsHidden))
        {
            videoStreamConfigs.Add(new VideoStreamConfig
            {
                Id = videoStream.Id,
                M3UFileId = videoStream.M3UFileId,
                User_Tvg_name = videoStream.User_Tvg_name,
                Tvg_ID = videoStream.Tvg_ID,
                User_Tvg_ID = videoStream.User_Tvg_ID,
                User_Tvg_Logo = videoStream.User_Tvg_logo,
                User_Tvg_chno = videoStream.User_Tvg_chno,
                TimeShift = videoStream.TimeShift,
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