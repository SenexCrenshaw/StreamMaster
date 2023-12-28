using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;

using System.Net;
using System.Xml;
using System.Xml.Serialization;

using static StreamMaster.Domain.Common.GetStreamGroupEPGHandler;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupEPG(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupEPGValidator : AbstractValidator<GetStreamGroupEPG>
{
    public GetStreamGroupEPGValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupEPGHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupEPG> logger, ISchedulesDirectDataService schedulesDirectDataService, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroupEPG, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly ParallelOptions parallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };

    public string GetIconUrl(string iconOriginalSource, Setting setting)
    {
        string url = _httpContextAccessor.GetUrl();

        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            return $"{url}{setting.DefaultIcon}";
        }

        string originalUrl = iconOriginalSource;

        if (iconOriginalSource.StartsWith('/'))
        {
            iconOriginalSource = iconOriginalSource[1..];
        }

        if (iconOriginalSource.StartsWith("images/"))
        {
            return $"{url}/{iconOriginalSource}";
        }
        else if (!iconOriginalSource.StartsWith("http"))
        {
            return GetApiUrl(SMFileTypes.TvLogo, originalUrl);
        }
        else if (setting.CacheIcons)
        {
            return iconOriginalSource.StartsWith("https://json.schedulesdirect.org")
                ? GetApiUrl(SMFileTypes.SDImage, originalUrl)
                : GetApiUrl(SMFileTypes.Icon, originalUrl);
        }

        return iconOriginalSource;
    }

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupEPG request, CancellationToken cancellationToken)
    {

        Setting settings = MemoryCache.GetSetting();

        List<VideoStreamDto> videoStreams = [];

        videoStreams = request.StreamGroupId == 0
            ? await Repository.VideoStream.GetVideoStreams()
            : await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.StreamGroupId, cancellationToken);

        List<VideoStreamConfig> videoStreamConfigs = [];

        foreach (VideoStreamDto? videoStream in videoStreams.Where(a => !a.IsHidden))
        {
            videoStreamConfigs.Add(new VideoStreamConfig
            {
                Id = videoStream.Id,
                M3UFileId = videoStream.M3UFileId,
                User_Tvg_name = videoStream.User_Tvg_name,
                User_Tvg_ID = videoStream.User_Tvg_ID,
                User_Tvg_Logo = videoStream.User_Tvg_logo,
                User_Tvg_chno = videoStream.User_Tvg_chno,
                IsDuplicate = false,
                IsDummy = false
            });
        }


        HashSet<string> epgids = [];

        foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs)
        {
            videoStreamConfig.IsDummy = videoStreamConfig.User_Tvg_ID.Equals("DUMMY");
            if (epgids.Contains(videoStreamConfig.User_Tvg_ID))
            {
                videoStreamConfig.IsDuplicate = true;
            }
            else
            {
                epgids.Add(videoStreamConfig.User_Tvg_ID);
            }
        }

        XMLTV epgData = schedulesDirectDataService.CreateXmlTv(_httpContextAccessor.GetUrl(), videoStreamConfigs) ?? new XMLTV();

        return SerializeXMLTVData(epgData);
    }

    private static string SerializeXMLTVData(XMLTV xmltv)
    {
        XmlSerializerNamespaces ns = new();
        ns.Add("", "");

        // Create a Utf8StringWriter
        using Utf8StringWriter textWriter = new();

        XmlWriterSettings settings = new()
        {
            Indent = true,
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineChars = "\n"
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