using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupM3U2(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupM3U2Validator : AbstractValidator<GetStreamGroupM3U2>
{
    public GetStreamGroupM3U2Validator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupM3U2Handler : IRequestHandler<GetStreamGroupM3U2, string>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ISender _sender;
    private readonly object Lock = new();

    public GetStreamGroupM3U2Handler(
           IMapper mapper,
           IMemoryCache memoryCache,
            ISender sender,
        IAppDbContext context)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public async Task<string> Handle(GetStreamGroupM3U2 command, CancellationToken cancellationToken)
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

        SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        ParallelOptions po = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = System.Environment.ProcessorCount
        };

        ConcurrentDictionary<int, string> retlist = new();

        //List<IconFileDto> icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);
        var icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

        _ = Parallel.ForEach(videoStreams.OrderBy(a => a.User_Tvg_chno), po, (videoStream, state, longCid) =>
        {
            int cid = Convert.ToInt32(longCid);

            if (command.StreamGroupNumber == 0 && videoStream.User_Tvg_chno == 0)
            {
                videoStream.User_Tvg_chno = cid;
            }

            IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStream.User_Tvg_logo);
            string Logo = icon != null ? icon.Source : setting.BaseHostURL + setting.DefaultIcon;

            videoStream.User_Tvg_logo = Logo;
            string url = $"{setting.BaseHostURL}api/streamgroups/{command.StreamGroupNumber}/stream/{videoStream.Id}";

            string ttt = $"#EXTINF:0 CUID=\"{videoStream.CUID}\" tvg-name=\"{videoStream.User_Tvg_name}\" ";
            ttt += $"tvg-id=\"{videoStream.User_Tvg_ID}\" group-title=\"{videoStream.User_Tvg_group}\"";
            ttt += $",{videoStream.User_Tvg_name}\r\n";
            ttt += $"{url}\r\n";

            _ = retlist.TryAdd(videoStream.User_Tvg_chno, ttt);
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
