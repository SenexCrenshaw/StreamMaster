using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Authentication;
using StreamMasterDomain.Configuration;
using StreamMasterDomain.Dto;

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
    private readonly IConfigFileProvider _configFileProvider;
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    private readonly ISender _sender;

    public GetStreamGroupM3UHandler(
           IMapper mapper,
           IHttpContextAccessor httpContextAccessor,
            ISender sender,
             IConfigFileProvider configFileProvider,
        IAppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _configFileProvider = configFileProvider;

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
               
        var crypt = Path.GetFileNameWithoutExtension(requestPath);
        var iv = crypt.GetIV(_configFileProvider.Setting.ServerKey,128);
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
            string Logo = icon != null ? icon.Source : "/" + _configFileProvider.Setting.DefaultIcon;

            videoStream.User_Tvg_logo = Logo;

            var encodedNumbers = command.StreamGroupNumber.EncodeValues128(videoStream.Id, _configFileProvider.Setting.ServerKey, iv);

            string url = GetUrl();
            var videoUrl = $"{url}/api/streamgroups/stream/{encodedNumbers}";

            //if (!string.IsNullOrEmpty(_configFileProvider.Setting.APIPassword) && !string.IsNullOrEmpty(_configFileProvider.Setting.APIUserName))
            //{
            //    url += "?apiusername=" + _configFileProvider.Setting.APIUserName + "&apipassword=" + _configFileProvider.Setting.APIPassword;
            //}

            string ttt = $"#EXTINF:0 CUID=\"{videoStream.CUID}\" channel-id=\"{videoStream.CUID}\" channel-number=\"{videoStream.User_Tvg_chno}\" tvg-name=\"{videoStream.User_Tvg_name}\" tvg-chno=\"{videoStream.User_Tvg_chno}\" ";
            ttt += $"tvg-id=\"{videoStream.User_Tvg_ID}\" tvg-logo=\"{videoStream.User_Tvg_logo}\" group-title=\"{videoStream.User_Tvg_group}\"";
            ttt += $",{videoStream.User_Tvg_name}\r\n";
            ttt += $"{videoUrl}\r\n";

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

    private string GetUrl()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var scheme = request.Scheme;
        var host = request.Host;

        var url = $"{scheme}://{host}";

        return url;
    }
}
