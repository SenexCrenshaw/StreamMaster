using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities.EPG;

using System.Web;

namespace StreamMasterApplication.Icons.Commands;

public class BuildProgIconsCacheFromEPGsRequest : IRequest<bool>
{
}

public class BuildProgIconsCacheFromEPGsRequestHandler : IRequestHandler<BuildProgIconsCacheFromEPGsRequest, bool>
{
    private readonly IAppDbContext _context;

    private readonly ILogger<BuildProgIconsCacheFromEPGsRequestHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ISender _sender;

    public BuildProgIconsCacheFromEPGsRequestHandler(
        ILogger<BuildProgIconsCacheFromEPGsRequestHandler> logger,
        IMemoryCache memoryCache,
          IMapper mapper,

         IAppDbContext context, ISender sender)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public async Task<bool> Handle(BuildProgIconsCacheFromEPGsRequest command, CancellationToken cancellationToken)
    {
        var setting = FileUtil.GetSetting();

        var epgFiles = _context.EPGFiles.ToList();
        foreach (var epg in epgFiles)
        {
            var tv = await epg.GetTV();
            if (tv == null)
            {
                continue;
            }

            //fix me. channellogo should have the channel name
            var epgChannels = tv.Channel.Where(a => a != null && a.Icon != null && a.Icon.Src != null && a.Icon.Src != "");
            //var epgIcons = epgChannels.Select(a => new { a.Displayname, a.Icon.Src }).ToList();

            if (!epgChannels.Any()) { continue; }

            await WorkOnIcons(epg.Id, FileDefinitions.Icon, epgChannels, cancellationToken).ConfigureAwait(false);
        }

        //IEnumerable<StreamMasterDomain.Entities.EPG.Programme> _programmes = _memoryCache.Programmes();

        //icons = _programmes
        //   .Where(a => a is not null && a.Icon is not null && a.Icon.Count > 0 && a.Icon.Any(a => a.Src is not null))
        //   .SelectMany(a => a.Icon.Where(a => a.Src is not null).Select(a => a.Src))
        //   .Distinct().ToList();

        //var test = icons.FirstOrDefault(a => a.Contains("deba6af644347122056ec73f6b885215ff4534230b214addfc795ae7db60c38f"));

        //if (!icons.Any()) { return false; }

        //await WorkOnIcons(FileDefinitions.ProgrammeIcon, icons, _setting, cancellationToken).ConfigureAwait(false);

        return true;
    }

    private async Task WorkOnIcons(int epgFileId, FileDefinition fd, IEnumerable<TvChannel> channels, CancellationToken cancellationToken)
    {
        foreach (var channel in channels)
        {
            if (channel is null || channel.Icon == null || string.IsNullOrEmpty(channel.Icon.Src) || channel.Displayname == null || channel.Displayname[0] == null || channel.Displayname[0] == "")
            {
                continue;
            }

            if (cancellationToken.IsCancellationRequested) { return; }
            var icon = channel.Icon.Src;
            string source = HttpUtility.UrlDecode(icon);

            if (source.ToLower().StartsWith("https://json.schedulesdirect.org/20141201/image/"))
            {
            }
            else
            {
                string name = channel.Displayname != null ? channel.Displayname[0].ToString() : Path.GetFileNameWithoutExtension(source);
                var iconDto = await IconHelper.AddIcon(source, name, _mapper, _memoryCache, fd, cancellationToken);
                if (iconDto is null)
                {
                    continue;
                }

                var channelLogos = _memoryCache.ChannelLogos();
                if (!channelLogos.Any(a => a.LogoUrl == source))
                {
                    var cl = new ChannelLogoDto { LogoUrl = source, ChannelName = channel.Displayname[0].ToString(), EPGFileId = epgFileId };

                    _memoryCache.Add(cl);
                }
            }
        }
    }
}
