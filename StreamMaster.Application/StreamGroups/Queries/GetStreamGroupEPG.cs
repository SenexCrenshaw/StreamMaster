using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Requests;
using StreamMaster.SchedulesDirect.Helpers;

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

public class GetStreamGroupEPGHandler(IHttpContextAccessor httpContextAccessor, IEPGHelper epgHelper, IXMLTVBuilder xMLTVBuilder, ILogger<GetStreamGroupEPG> logger, ISchedulesDirectDataService schedulesDirectDataService, IRepositoryWrapper Repository, IMemoryCache MemoryCache)
    : IRequestHandler<GetStreamGroupEPG, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly ParallelOptions parallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupEPG request, CancellationToken cancellationToken)
    {

        Setting settings = MemoryCache.GetSetting();

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
                // Initialize a variable to hold the service
                MxfService? service = null;

                // Check if User_Tvg_ID is not empty and assign the corresponding service
                if (!string.IsNullOrEmpty(videoStreamConfig.User_Tvg_ID))
                {
                    service = allservices.Find(a => a.StationId.Equals(videoStreamConfig.User_Tvg_ID, StringComparison.CurrentCultureIgnoreCase));
                }
                // If service is still null and Tvg_ID is not empty, assign the corresponding service
                else if (!string.IsNullOrEmpty(videoStreamConfig.Tvg_ID))
                {
                    service = allservices.Find(a => a.StationId.Equals(videoStreamConfig.Tvg_ID, StringComparison.CurrentCultureIgnoreCase));
                }

                // If a service is found, update User_Tvg_ID accordingly
                if (service != null)
                {
                    videoStreamConfig.User_Tvg_ID = $"{service.EPGNumber}-{service.StationId}";
                    videoStreamConfig.IsDummy = false;
                }
                else
                {

                    videoStreamConfig.User_Tvg_ID = EPGHelper.DummyId + "-" + videoStreamConfig.Id;

                    dummyData.FindOrCreateDummyService(videoStreamConfig.Id, videoStreamConfig);
                }
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
        Setting setting = MemoryCache.GetSetting();
        XmlSerializerNamespaces ns = new();
        ns.Add("", "");

        // Create a Utf8StringWriter
        using Utf8StringWriter textWriter = new();

        XmlWriterSettings settings = new()
        {
            Indent = setting.PrettyEPG,
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.None,
            //NewLineChars = "\n"
        };

        // Create an XmlWriter using Utf8StringWriter
        using XmlWriter writer = XmlWriter.Create(textWriter, settings);

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