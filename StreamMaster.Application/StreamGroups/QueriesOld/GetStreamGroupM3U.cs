using FluentValidation;

using Microsoft.AspNetCore.Http;

using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Web;

namespace StreamMaster.Application.StreamGroups.QueriesOld;

[RequireAll]
public record GetStreamGroupM3U(int StreamGroupId, bool UseSMChannelId) : IRequest<string>;

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
        if (iv == null && !request.UseSMChannelId)
        {
            return DefaultReturn;
        }
        this.iv = iv;

        List<SMChannel> smChannels = await Repository.SMChannel.GetSMChannelsFromStreamGroup(request.StreamGroupId);

        if (!smChannels.Any())
        {
            return DefaultReturn;
        }

        // Initialize the ConcurrentBag with distinct channel numbers
        existingChNos = new ConcurrentBag<int>(smChannels.Select(a => a.ChannelNumber).Distinct());

        // Retrieve necessary data in parallel
        var videoStreamData = smChannels
     .AsParallel()
     .WithDegreeOfParallelism(Environment.ProcessorCount)
     .Select((smChannel, index) =>
     {
         (int ChNo, string m3uLine) = BuildM3ULineForVideoStream(smChannel, url, request, index, settings);
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

    private (int ChNo, string m3uLine) BuildM3ULineForVideoStream(SMChannel smChannel, string url, GetStreamGroupM3U request, int cid, Setting setting)
    {

        string epgChannelId;

        string channelId = string.Empty;
        string tvgID = string.Empty;

        if (string.IsNullOrEmpty(smChannel.EPGId))
        {
            epgChannelId = smChannel.Group;
        }
        else
        {
            if (EPGHelper.IsValidEPGId(smChannel.EPGId))
            {
                (_, epgChannelId) = smChannel.EPGId.ExtractEPGNumberAndStationId();
                MxfService? service = schedulesDirectDataService.GetService(smChannel.EPGId);
                if (setting.M3UUseCUIDForChannelID)
                {
                    tvgID = epgChannelId;
                    channelId = smChannel.Id.ToString();
                }
                else
                {
                    tvgID = service?.CallSign ?? epgChannelId;
                    channelId = tvgID;
                }
            }
            else
            {
                epgChannelId = tvgID = channelId = smChannel.EPGId;
            }
        }

        string name = smChannel.Name;

        //if (setting.M3UIgnoreEmptyEPGID)
        //{
        //    showM3UFieldTvgId = !(string.IsNullOrEmpty(videoStream.EPGId) && string.IsNullOrEmpty(smChannelDto.EPGId));
        //}


        if (request.StreamGroupId == 1 && smChannel.ChannelNumber == 0)
        {
            smChannel.ChannelNumber = cid;
        }

        string logo = GetIconUrl(smChannel.Logo, setting);
        smChannel.Logo = logo;

        string videoUrl;
        if (request.UseSMChannelId)
        {
            videoUrl = $"{url}/v/v/{smChannel.SMChannelId}";
        }
        else
        {
            if (hlssettings.HLSM3U8Enable)
            {
                videoUrl = $"{url}/api/stream/{smChannel.Id}.m3u8";
            }
            else
            {
                string encodedName = HttpUtility.HtmlEncode(smChannel.Name).Trim()
                         .Replace("/", "")
                         .Replace(" ", "_");

                string encodedNumbers = request.StreamGroupId.EncodeValues128(smChannel.Id, setting.ServerKey, iv);
                videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";
            }

        }

        if (setting.M3UUseChnoForId)
        {
            tvgID = smChannel.ChannelNumber.ToString();
            if (!setting.M3UUseCUIDForChannelID)
            {
                channelId = tvgID;
            }
        }

        List<string> fieldList = [$"#EXTINF:0 CUID=\"{smChannel.Id}\""];

        fieldList.Add($"tvg-name=\"{name}\"");

        fieldList.Add($"channel-id=\"{channelId}\"");
        fieldList.Add($"tvg-id=\"{tvgID}\"");

        fieldList.Add($"tvg-logo=\"{smChannel.Logo}\"");

        int chNo = GetNextChNo(smChannel.ChannelNumber);
        fieldList.Add($"tvg-chno=\"{chNo}\"");
        fieldList.Add($"channel-number=\"{chNo}\"");


        if (setting.M3UStationId)
        {
            string toDisplay = string.IsNullOrEmpty(smChannel.StationId) ? epgChannelId : smChannel.StationId;
            fieldList.Add($"tvc-guide-stationid=\"{toDisplay}\"");
        }

        if (setting.M3UFieldGroupTitle)
        {
            if (!string.IsNullOrEmpty(smChannel.GroupTitle))
            {
                fieldList.Add($"group-title=\"{smChannel.GroupTitle}\"");
            }
            else
            {
                fieldList.Add($"group-title=\"{smChannel.Group}\"");
            }
        }


        string lines = string.Join(" ", [.. fieldList.Order()]);
        lines += $",{smChannel.Name}\r\n";
        lines += $"{videoUrl}";

        return (chNo, lines);
    }

    private string GetApiUrl(SMFileTypes path, string source)
    {
        string url = httpContextAccessor.GetUrl();
        return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }

}