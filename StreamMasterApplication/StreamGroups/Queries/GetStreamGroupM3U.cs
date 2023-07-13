using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System;
using System.Collections.Concurrent;

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
    private readonly Setting _setting;
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

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

    public async Task<string> Handle(GetStreamGroupM3U command, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> videoStreams = new();
        if (command.StreamGroupNumber > 0)
        {
            StreamGroupDto? sg = await _sender.Send(new GetStreamGroupByStreamNumber(command.StreamGroupNumber), cancellationToken).ConfigureAwait(false);
            if (sg == null)
            {
                return "";
            }
            videoStreams = sg.VideoStreams.Where(a => !a.IsHidden).ToList();
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

        // List<IconFileDto> icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);
        var icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

        var requestPath = _httpContextAccessor.HttpContext.Request.Path.Value.ToString();
        var iv = requestPath.GetIVFromPath(128);
        if (iv == null)
        {
            return "";
        }

        _ = Parallel.ForEach(videoStreams.OrderBy(a => a.User_Tvg_chno), po, (videoStream, state, longCid) =>
        {
            int cid = Convert.ToInt32(longCid);

            if (command.StreamGroupNumber == 0 && videoStream.User_Tvg_chno == 0)
            {
                videoStream.User_Tvg_chno = cid;
            }

            IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStream.User_Tvg_logo);
            string Logo = icon != null ? icon.Source : "/" + _setting.DefaultIcon;

            videoStream.User_Tvg_logo = Logo;

            string videoUrl = videoStream.Url;

            string url = _httpContextAccessor.GetUrl();

            var encodedNumbers = command.StreamGroupNumber.EncodeValues128(videoStream.Id, _setting.ServerKey, iv);

            videoUrl = $"{url}/api/streamgroups/stream/{encodedNumbers}/{videoStream.User_Tvg_name.Replace(" ", "_")}";

            var fieldList = new List<string>();

            fieldList.Add($"#EXTINF:0 CUID=\"{videoStream.CUID}\"");

            if (_setting.M3UFieldChannelId)
            {
                fieldList.Add($"channel-id=\"{videoStream.CUID}\"");
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
                if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                {
                    if (_setting.UseDummyEPGForBlanks)
                    {
                        fieldList.Add($"tvg-id=\"{videoStream.User_Tvg_ID}\"");
                    }
                }
                else
                {
                    fieldList.Add($"tvg-id=\"{videoStream.User_Tvg_ID}\"");
                }
            }

            if (_setting.M3UFieldTvgLogo)
            {
                fieldList.Add($"tvg-logo=\"{url}{videoStream.User_Tvg_logo}\"");
            }
            if (_setting.M3UFieldGroupTitle)
            {
                fieldList.Add($"group-title=\"{videoStream.User_Tvg_group}\"");
            }
            var lines = string.Join(" ", fieldList.ToArray());

            lines +=$",{videoStream.User_Tvg_name}\r\n";
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
}