using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Authentication;
using StreamMasterDomain.Dto;

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

public class GetStreamGroupM3UHandler : IRequestHandler<GetStreamGroupM3U, string>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly Setting _setting;

    public GetStreamGroupM3UHandler(
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ISender sender,
        IAppDbContext context
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _setting = FileUtil.GetSetting();

        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public string GetIconUrl(string iconOriginalSource)
    {
        string url = _httpContextAccessor.GetUrl();

        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            iconOriginalSource = $"{url}{_setting.DefaultIcon}";
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
        else if (_setting.CacheIcons)
        {
            iconOriginalSource = GetApiUrl(SMFileTypes.Icon, originalUrl);
        }

        return iconOriginalSource;
    }

    public async Task<string> Handle(GetStreamGroupM3U command, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> videoStreams = new();
        string url = _httpContextAccessor.GetUrl();

        if (command.StreamGroupNumber > 0)
        {
            StreamGroupDto? sg = await _context.GetStreamGroupDtoByStreamGroupNumber(command.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);
            if (sg == null)
            {
                return "";
            }
            videoStreams = sg.ChildVideoStreams.Where(a => !a.IsHidden).ToList();
        }
        else
        {
            videoStreams = _context.VideoStreams
                .Where(a => !a.IsHidden)
                .AsNoTracking()
                .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                .ToList();
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

        var icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

        var requestPath = _httpContextAccessor.HttpContext.Request.Path.Value.ToString();
        var iv = requestPath.GetIVFromPath(128);
        if (iv == null)
        {
            return "";
        }

        _ = Parallel.ForEach(videoStreams.OrderBy(a => a.User_Tvg_chno), po, (videoStream, state, longCid) =>
        {
            if (_setting.M3UFieldTvgId)
            {
                if (_setting.M3UIgnoreEmptyEPGID && string.IsNullOrEmpty(videoStream.User_Tvg_ID))
                {
                    return;
                }
            }

            int cid = Convert.ToInt32(longCid);

            if (command.StreamGroupNumber == 0 && videoStream.User_Tvg_chno == 0)
            {
                videoStream.User_Tvg_chno = cid;
            }

            var logo = GetIconUrl(videoStream.User_Tvg_logo);

            videoStream.User_Tvg_logo = logo;

            string videoUrl = videoStream.Url;

            var encodedNumbers = command.StreamGroupNumber.EncodeValues128(videoStream.Id, _setting.ServerKey, iv);

            var encodedName = HttpUtility.HtmlEncode(videoStream.User_Tvg_name).Trim().Replace(" ", "_");
            videoUrl = $"{url}/api/videostreams/stream/{encodedNumbers}/{encodedName}";

            var fieldList = new List<string>
            {
                $"#EXTINF:0 CUID=\"{videoStream.Id}\""
            };

            if (_setting.M3UFieldChannelId)
            {
                fieldList.Add($"channel-id=\"{videoStream.Id}\"");
            }

            if (_setting.M3UFieldChannelNumber)
            {
                fieldList.Add($"channel-number=\"{videoStream.User_Tvg_chno}\"");
            }

            if (_setting.M3UFieldTvgName)
            {
                fieldList.Add($"tvg-name=\"{videoStream.User_Tvg_name}\"");
            }

            if (_setting.M3UFieldTvgChno)
            {
                fieldList.Add($"tvg-chno=\"{videoStream.User_Tvg_chno}\"");
            }

            if (_setting.M3UFieldTvgId)
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

            if (_setting.M3UFieldTvgLogo)
            {
                fieldList.Add($"tvg-logo=\"{videoStream.User_Tvg_logo}\"");
            }
            if (_setting.M3UFieldGroupTitle)
            {
                fieldList.Add($"group-title=\"{videoStream.User_Tvg_group}\"");
            }
            var lines = string.Join(" ", fieldList.ToArray());

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

        if (!string.IsNullOrEmpty(_setting.DummyRegex))
        {
            Regex regex = new(_setting.DummyRegex, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            var test = regex.IsMatch(videoStream.User_Tvg_ID);
            return test;
        }

        return false;
    }
}
