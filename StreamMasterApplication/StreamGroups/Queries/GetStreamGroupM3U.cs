using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Extensions;

using StreamMasterDomain.Authentication;

using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Web;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupM3U(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupM3UValidator : AbstractValidator<GetStreamGroupM3U>
{
    public GetStreamGroupM3UValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}


public class GetStreamGroupM3UHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupM3U> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroupM3U, string>
{
    public string GetIconUrl(string iconOriginalSource, Setting setting)
    {
        string url = httpContextAccessor.GetUrl();

        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            iconOriginalSource = $"{url}{setting.DefaultIcon}";
            return iconOriginalSource;
        }

        string originalUrl = iconOriginalSource;

        if (iconOriginalSource.StartsWith('/'))
        {
            iconOriginalSource = iconOriginalSource[1..];
        }

        if (iconOriginalSource.StartsWith("images/"))
        {
            iconOriginalSource = $"{url}/{iconOriginalSource}";
        }
        else if (!iconOriginalSource.StartsWith("http"))
        {
            iconOriginalSource = GetApiUrl(SMFileTypes.TvLogo, originalUrl);
        }
        else if (setting.CacheIcons)
        {
            iconOriginalSource = GetApiUrl(SMFileTypes.Icon, originalUrl);
        }

        return iconOriginalSource;
    }
    private byte[] iv = [];

    private const string DefaultReturn = "#EXTM3U\r\n";

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupM3U request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync();
        string url = httpContextAccessor.GetUrl();
        string requestPath = httpContextAccessor.HttpContext.Request.Path.Value.ToString();
        byte[]? iv = requestPath.GetIVFromPath(setting.ServerKey, 128);
        if (iv == null)
        {
            return DefaultReturn;
        }
        this.iv = iv;

        List<VideoStreamDto> videoStreams = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.StreamGroupId, cancellationToken);

        if (!videoStreams.Any())
        {
            return DefaultReturn;
        }

        // Retrieve necessary data in parallel
        var videoStreamData = videoStreams
     .AsParallel()
     .WithDegreeOfParallelism(Environment.ProcessorCount)
     .Select((videoStream, index) =>
     {
         return new
         {
             VideoStream = videoStream,
             M3ULine = BuildM3ULineForVideoStream(videoStream, url, request, index, setting)
         };
     }).ToList();

        ConcurrentDictionary<int, string> retlist = new();

        // Process the data serially
        foreach (var data in videoStreamData.OrderBy(a => a.VideoStream.User_Tvg_chno))
        {
            if (!string.IsNullOrEmpty(data.M3ULine))
            {
                retlist.TryAdd(data.VideoStream.User_Tvg_chno, data.M3ULine);
            }
        }

        return AssembleReturnString(retlist);
    }

    private string BuildM3ULineForVideoStream(VideoStreamDto videoStream, string url, GetStreamGroupM3U request, int cid, Setting setting)
    {
        bool showM3UFieldTvgId = setting.M3UFieldTvgId;

        bool isUserTvgIdInvalid = string.IsNullOrEmpty(videoStream.User_Tvg_ID)
                      || StringComparer.OrdinalIgnoreCase.Equals(videoStream.User_Tvg_ID, "dummy");

        if (setting.M3UIgnoreEmptyEPGID && isUserTvgIdInvalid)
        {
            if (setting.M3UFieldTvgId)
            {
                showM3UFieldTvgId = false;
            }
            else
            {
                return "";
            }
        }

        //int cid = Convert.ToInt32(longCid);

        if (request.StreamGroupId == 1 && videoStream.User_Tvg_chno == 0)
        {
            videoStream.User_Tvg_chno = cid;
        }

        string logo = GetIconUrl(videoStream.User_Tvg_logo, setting);
        videoStream.User_Tvg_logo = logo;

        string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim()
                .Replace("/", "")
                .Replace(" ", "_");

        string encodedNumbers = request.StreamGroupId.EncodeValues128(videoStream.Id, setting.ServerKey, iv);
        string videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

        List<string> fieldList =
    [
        $"#EXTINF:0 CUID=\"{videoStream.Id}\""
    ];

        if (setting.M3UFieldChannelId)
        {
            fieldList.Add($"channel-id=\"{videoStream.Id}\"");
        }

        if (setting.M3UFieldChannelNumber)
        {
            fieldList.Add($"channel-number=\"{videoStream.User_Tvg_chno}\"");
        }

        if (setting.M3UFieldTvgName)
        {
            fieldList.Add($"tvg-name=\"{videoStream.User_Tvg_name}\"");
        }

        if (setting.M3UFieldTvgChno)
        {
            fieldList.Add($"tvg-chno=\"{videoStream.User_Tvg_chno}\"");
        }

        if (showM3UFieldTvgId)
        {
            fieldList.Add($"tvg-id=\"{videoStream.User_Tvg_ID}\"");
        }

        if (setting.M3UFieldTvgLogo)
        {
            fieldList.Add($"tvg-logo=\"{videoStream.User_Tvg_logo}\"");
        }
        if (setting.M3UFieldGroupTitle)
        {
            if (!string.IsNullOrEmpty(videoStream.GroupTitle))
            {
                fieldList.Add($"group-title=\"{videoStream.GroupTitle}\"");
            }
            else
            {
                fieldList.Add($"group-title=\"{videoStream.User_Tvg_group}\"");
            }

        }

        string lines = string.Join(" ", fieldList.ToArray());
        lines += $",{videoStream.User_Tvg_name}\r\n";
        lines += $"{videoUrl}";

        return lines;
    }


    [LogExecutionTimeAspect]
    private string AssembleReturnString(ConcurrentDictionary<int, string> retlist)
    {
        StringBuilder ret = new("#EXTM3U\r\n");
        foreach (int rl in retlist.Keys.Order())
        {
            if (retlist.TryGetValue(rl, out string? str) && !string.IsNullOrEmpty(str))
            {
                ret.AppendLine(str);
            }
        }
        return ret.ToString();
    }

    private string GetApiUrl(SMFileTypes path, string source)
    {
        string url = httpContextAccessor.GetUrl();
        return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }

}