using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Domain.Authentication;
using StreamMaster.SchedulesDirect.Domain.Enums;

using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Web;

namespace StreamMaster.Application.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupM3U(int StreamGroupId, bool UseShortId) : IRequest<string>;

public class GetStreamGroupM3UValidator : AbstractValidator<GetStreamGroupM3U>
{
    public GetStreamGroupM3UValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupM3UHandler(IHttpContextAccessor httpContextAccessor, ISchedulesDirectDataService schedulesDirectDataService, IEPGHelper epgHelper, ILogger<GetStreamGroupM3U> logger, IRepositoryWrapper Repository, IOptionsMonitor<Setting> intsettings, IOptionsMonitor<HLSSettings> inthlssettings)
    : IRequestHandler<GetStreamGroupM3U, string>
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;

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
    private readonly ConcurrentBag<int> chNos = [];
    private ConcurrentBag<int> existingChNos = [];

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupM3U request, CancellationToken cancellationToken)
    {

        string url = httpContextAccessor.GetUrl();
        string requestPath = httpContextAccessor.HttpContext.Request.Path.Value.ToString();
        byte[]? iv = requestPath.GetIVFromPath(settings.ServerKey, 128);
        if (iv == null && !request.UseShortId)
        {
            return DefaultReturn;
        }
        this.iv = iv;

        List<VideoStreamDto> videoStreams = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.StreamGroupId, cancellationToken);

        if (!videoStreams.Any())
        {
            return DefaultReturn;
        }

        // Initialize the ConcurrentBag with distinct channel numbers
        existingChNos = new ConcurrentBag<int>(videoStreams.Select(a => a.User_Tvg_chno).Distinct());

        // Retrieve necessary data in parallel
        var videoStreamData = videoStreams
     .AsParallel()
     .WithDegreeOfParallelism(Environment.ProcessorCount)
     .Select((videoStream, index) =>
     {
         (int ChNo, string m3uLine) = BuildM3ULineForVideoStream(videoStream, url, request, index, settings);
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

    private int GetNextChNo(int chNo)
    {
        if (chNos.Contains(chNo))
        {
            foreach (int num in existingChNos.Concat(chNos))
            {
                if (num != chNo)
                {
                    break;
                }
                chNo++;
            }
        }
        chNos.Add(chNo);

        return chNo;
    }

    private (int ChNo, string m3uLine) BuildM3ULineForVideoStream(VideoStreamDto videoStream, string url, GetStreamGroupM3U request, int cid, Setting setting)
    {

        string epgChannelId;

        string channelId = string.Empty;
        string tvgID = string.Empty;

        if (string.IsNullOrEmpty(videoStream.User_Tvg_ID))
        {
            epgChannelId = videoStream.User_Tvg_group;
        }
        else
        {
            if (EPGHelper.IsValidEPGId(videoStream.User_Tvg_ID))
            {
                (_, epgChannelId) = videoStream.User_Tvg_ID.ExtractEPGNumberAndStationId();
                MxfService? service = schedulesDirectDataService.GetService(videoStream.User_Tvg_ID);
                if (setting.M3UUseCUIDForChannelID)
                {
                    tvgID = epgChannelId;
                    channelId = videoStream.Id;
                }
                else
                {
                    tvgID = service?.CallSign ?? epgChannelId;
                    channelId = tvgID;
                }
            }
            else
            {
                epgChannelId = tvgID = channelId = videoStream.User_Tvg_ID;
            }
        }

        string name = videoStream.User_Tvg_name;

        //if (setting.M3UIgnoreEmptyEPGID)
        //{
        //    showM3UFieldTvgId = !(string.IsNullOrEmpty(videoStream.Tvg_ID) && string.IsNullOrEmpty(videoStream.User_Tvg_ID));
        //}


        if (request.StreamGroupId == 1 && videoStream.User_Tvg_chno == 0)
        {
            videoStream.User_Tvg_chno = cid;
        }

        string logo = GetIconUrl(videoStream.User_Tvg_logo, setting);
        videoStream.User_Tvg_logo = logo;

        string videoUrl;
        if (request.UseShortId)
        {
            videoUrl = $"{url}/v/v/{videoStream.ShortId}";
        }
        else
        {
            if (hlssettings.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{videoStream.Id}.m3u8";
            }
            else
            {
                string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim()
                         .Replace("/", "")
                         .Replace(" ", "_");

                string encodedNumbers = request.StreamGroupId.EncodeValues128(videoStream.Id, setting.ServerKey, iv);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";
            }

        }

        if (setting.M3UUseChnoForId)
        {
            tvgID = videoStream.User_Tvg_chno.ToString();
            if (!setting.M3UUseCUIDForChannelID)
            {
                channelId = tvgID;
            }
        }

        List<string> fieldList = [$"#EXTINF:0 CUID=\"{videoStream.Id}\""];

        fieldList.Add($"tvg-name=\"{name}\"");

        fieldList.Add($"channel-id=\"{channelId}\"");
        fieldList.Add($"tvg-id=\"{tvgID}\"");

        fieldList.Add($"tvg-logo=\"{videoStream.User_Tvg_logo}\"");

        int chNo = GetNextChNo(videoStream.User_Tvg_chno);
        fieldList.Add($"tvg-chno=\"{chNo}\"");
        fieldList.Add($"channel-number=\"{chNo}\"");


        if (setting.M3UStationId)
        {
            string toDisplay = string.IsNullOrEmpty(videoStream.StationId) ? epgChannelId : videoStream.StationId;
            fieldList.Add($"tvc-guide-stationid=\"{toDisplay}\"");
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


        string lines = string.Join(" ", [.. fieldList.Order()]);
        lines += $",{videoStream.User_Tvg_name}\r\n";
        lines += $"{videoUrl}";

        return (chNo, lines);
    }

    private string GetApiUrl(SMFileTypes path, string source)
    {
        string url = httpContextAccessor.GetUrl();
        return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }

}