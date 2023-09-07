using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Pagination;

using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupM3U(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupM3UValidator : AbstractValidator<GetStreamGroupM3U>
{
    public GetStreamGroupM3UValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupM3UHandler : BaseMemoryRequestHandler, IRequestHandler<GetStreamGroupM3U, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;


    public GetStreamGroupM3UHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupM3U> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, publisher, sender, hubContext, memoryCache) { _httpContextAccessor = httpContextAccessor; }


    public string GetIconUrl(string iconOriginalSource)
    {
        string url = _httpContextAccessor.GetUrl();

        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            iconOriginalSource = $"{url}{Settings.DefaultIcon}";
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
        else if (Settings.CacheIcons)
        {
            iconOriginalSource = GetApiUrl(SMFileTypes.Icon, originalUrl);
        }

        return iconOriginalSource;
    }

    public async Task<string> Handle(GetStreamGroupM3U request, CancellationToken cancellationToken)
    {
        IEnumerable<VideoStream> videoStreams;
        string url = _httpContextAccessor.GetUrl();

        if (request.StreamGroupNumber > 0)
        {
            StreamGroup? streamGroup = await Repository.StreamGroup
                    .FindAll()
                    .Include(a => a.ChildVideoStreams)
                    .FirstOrDefaultAsync(a => a.StreamGroupNumber == request.StreamGroupNumber, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            if (streamGroup == null)
            {
                return "";
            }
            videoStreams = streamGroup.ChildVideoStreams.Select(a => a.ChildVideoStream).Where(a => !a.IsHidden);
        }
        else
        {
            videoStreams = Repository.VideoStream.GetVideoStreamsHidden();
        }

        if (!videoStreams.Any())
        {
            return "";
        }

        ParallelOptions po = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        ConcurrentDictionary<int, string> retlist = new();

        IconFileParameters iconFileParameters = new();
        PagedResponse<IconFileDto> icons = await Sender.Send(new GetIcons(iconFileParameters), cancellationToken).ConfigureAwait(false);

        string requestPath = _httpContextAccessor.HttpContext.Request.Path.Value.ToString();
        byte[]? iv = requestPath.GetIVFromPath(128);
        if (iv == null)
        {
            return "";
        }

        _ = Parallel.ForEach(videoStreams.OrderBy(a => a.User_Tvg_chno), po, (videoStream, state, longCid) =>
        {
            bool showM3UFieldTvgId = Settings.M3UFieldTvgId;

            bool isUserTvgIdInvalid = string.IsNullOrEmpty(videoStream.User_Tvg_ID)
                          || StringComparer.OrdinalIgnoreCase.Equals(videoStream.User_Tvg_ID, "dummy");

            if (Settings.M3UIgnoreEmptyEPGID && isUserTvgIdInvalid)
            {
                if (Settings.M3UFieldTvgId)
                {
                    showM3UFieldTvgId = false;
                }
                else
                {
                    return;
                }
            }

            int cid = Convert.ToInt32(longCid);

            if (request.StreamGroupNumber == 0 && videoStream.User_Tvg_chno == 0)
            {
                videoStream.User_Tvg_chno = cid;
            }

            string logo = GetIconUrl(videoStream.User_Tvg_logo);

            videoStream.User_Tvg_logo = logo;

            string videoUrl = videoStream.Url;

            string encodedNumbers = request.StreamGroupNumber.EncodeValues128(videoStream.Id, Settings.ServerKey, iv);

            string encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim().Replace(" ", "_");
            videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            List<string> fieldList = new()
            {
                $"#EXTINF:0 CUID=\"{videoStream.Id}\""
            };

            if (Settings.M3UFieldChannelId)
            {
                fieldList.Add($"channel-id=\"{videoStream.Id}\"");
            }

            if (Settings.M3UFieldChannelNumber)
            {
                fieldList.Add($"channel-number=\"{videoStream.User_Tvg_chno}\"");
            }

            if (Settings.M3UFieldTvgName)
            {
                fieldList.Add($"tvg-name=\"{videoStream.User_Tvg_name}\"");
            }

            if (Settings.M3UFieldTvgChno)
            {
                fieldList.Add($"tvg-chno=\"{videoStream.User_Tvg_chno}\"");
            }

            if (showM3UFieldTvgId)
            {
                //if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                //{
                //    fieldList.Add($"tvg-id=\"{videoStream.User_Tvg_ID}\"");
                //}
                //else
                //{
                fieldList.Add($"tvg-id=\"{videoStream.User_Tvg_ID}\"");
                //}
            }

            if (Settings.M3UFieldTvgLogo)
            {
                fieldList.Add($"tvg-logo=\"{videoStream.User_Tvg_logo}\"");
            }
            if (Settings.M3UFieldGroupTitle)
            {
                fieldList.Add($"group-title=\"{videoStream.User_Tvg_group}\"");
            }
            string lines = string.Join(" ", fieldList.ToArray());

            lines += $",{videoStream.User_Tvg_name}\r\n";
            lines += $"{videoUrl}\r\n";

            _ = retlist.TryAdd(videoStream.User_Tvg_chno, lines);
        });

        string ret = "#EXTM3U\r\n";
        foreach (int rl in retlist.Keys.Order())
        {
            _ = retlist.TryGetValue(rl, out string? str);

            if (str != null)
            {
                ret += str;
            }
        }
        return ret;
    }

    private string GetApiUrl(SMFileTypes path, string source)
    {
        string url = _httpContextAccessor.GetUrl();
        return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }

    private bool IsVideoStreamADummy(VideoStreamDto videoStream)
    {
        if (string.IsNullOrEmpty(videoStream.User_Tvg_ID))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(Settings.DummyRegex))
        {
            Regex regex = new(Settings.DummyRegex, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            bool test = regex.IsMatch(videoStream.User_Tvg_ID);
            return test;
        }

        return false;
    }
}