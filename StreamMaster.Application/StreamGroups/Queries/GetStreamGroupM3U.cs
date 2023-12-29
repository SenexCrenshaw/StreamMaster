using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Authentication;

using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Web;

namespace StreamMaster.Application.StreamGroups.Queries;

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
    private HashSet<int> chNos = [];
    private HashSet<int> existingChNos = [];

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

        existingChNos = videoStreams.Select(a => a.User_Tvg_chno).Distinct().ToHashSet();

        // Retrieve necessary data in parallel
        var videoStreamData = videoStreams
     .AsParallel()
     .WithDegreeOfParallelism(Environment.ProcessorCount)
     .Select((videoStream, index) =>
     {
         var (ChNo, m3uLine) = BuildM3ULineForVideoStream(videoStream, url, request, index, setting);
         return new
         {
             ChNo,
             m3uLine
         };
     }).ToList();

        //ConcurrentDictionary<int, string> retlist = new();

        ////// Process the data serially
        //foreach (var data in videoStreamData.OrderBy(a => a.ChNo))
        //{
        //    if (!string.IsNullOrEmpty(data.m3uLine))
        //    {
        //        retlist.TryAdd(ChNo, data.m3uLine);
        //    }
        //}


        //   var ret = videoStreamData
        //.Select(c => new { VideoStream = c, IsNumeric = int.TryParse(c., out var num), NumericId = num })
        //.OrderBy(c => c.IsNumeric)
        //.ThenBy(c => c.NumericId)     
        //.Select(c => c.VideoStream)
        //.ToList();

        StringBuilder ret = new("#EXTM3U\r\n");
        foreach (var data in videoStreamData.OrderBy(a => a.ChNo))
        {
            if (!string.IsNullOrEmpty(data.m3uLine))
            {
                ret.AppendLine(data.m3uLine);
            }
        }

        return ret.ToString();
    }


    private (int ChNo, string m3uLine) BuildM3ULineForVideoStream(VideoStreamDto videoStream, string url, GetStreamGroupM3U request, int cid, Setting setting)
    {
        bool showM3UFieldTvgId = setting.M3UFieldTvgId;

        (int epgNumber, string stationId) = videoStream.User_Tvg_ID.ExtractEPGNumberAndStationId();

        bool isUserTvgIdInvalid = string.IsNullOrEmpty(stationId)
                      || StringComparer.OrdinalIgnoreCase.Equals(stationId, "dummy");

        if (setting.M3UIgnoreEmptyEPGID && isUserTvgIdInvalid)
        {
            if (setting.M3UFieldTvgId)
            {
                showM3UFieldTvgId = false;
            }
            else
            {
                return (0, "");
            }
        }


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


        if (setting.M3UFieldTvgName)
        {
            fieldList.Add($"tvg-name=\"{videoStream.User_Tvg_name}\"");
        }

        int chNo = videoStream.User_Tvg_chno;
        if (chNos.Contains(chNo))
        {
            foreach (var num in existingChNos.Concat(chNos))
            {
                if (num != chNo)
                {
                    break;
                }
                chNo++;
            }
        }
        chNos.Add(chNo);

        if (setting.M3UFieldTvgChno)
        {

            fieldList.Add($"tvg-chno=\"{chNo}\"");
        }

        if (setting.M3UFieldChannelNumber)
        {

            fieldList.Add($"channel-number=\"{chNo}\"");
        }

        if (showM3UFieldTvgId)
        {
            fieldList.Add($"tvg-id=\"{chNo}\"");
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
                fieldList.Add($"tvc-guide-categories\r\n=\"{videoStream.GroupTitle.Replace(';', ',')}\"");
            }
            else
            {
                fieldList.Add($"group-title=\"{videoStream.User_Tvg_group}\"");
            }

        }

        if (epgNumber == 0)
        {
            fieldList.Add($"tvc-guide-stationid=\"{stationId}\"");
        }


        string lines = string.Join(" ", fieldList.ToArray());
        lines += $",{videoStream.User_Tvg_name}\r\n";
        lines += $"{videoUrl}";

        return (chNo, lines);
    }

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