using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI;

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

            var epgChannels = tv.Channel.Where(a => a != null && a.Icon != null && a.Icon.Src != null && a.Icon.Src != "");

            if (!epgChannels.Any()) { continue; }

            await WorkOnEPGChannelIcons(epg.Id, epgChannels, cancellationToken).ConfigureAwait(false);
        }

        await WorkOnProgrammeIcons(cancellationToken).ConfigureAwait(false);

        return true;
    }

    private async Task WorkOnEPGChannelIcons(int epgFileId, IEnumerable<TvChannel> channels, CancellationToken cancellationToken)
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
                var aaa = 1;
            }
            else
            {
                string name = channel.Displayname != null ? channel.Displayname[0].ToString() : Path.GetFileNameWithoutExtension(source);
                var iconDto =  IconHelper.AddIcon(source, name, epgFileId, _mapper, _memoryCache, FileDefinitions.ChannelIcon, cancellationToken);
                if (iconDto is null)
                {
                    continue;
                }

                var channelLogos = _memoryCache.ChannelLogos();
                if (!channelLogos.Any(a => a.LogoUrl == source))
                {
                    var cl = new ChannelLogoDto { LogoUrl = source, EPGId = channel.Id, EPGFileId = epgFileId };

                    _memoryCache.Add(cl);
                }
            }
        }
    }

    private async Task WorkOnProgrammeIcons(CancellationToken cancellationToken)
    {
        var sgs = await _context.GetStreamGroupDtos("", cancellationToken).ConfigureAwait(false);
        var epgids = sgs.SelectMany(x => x.ChildVideoStreams.Select(a => a.User_Tvg_ID)).Distinct();

        List<Programme> programmes = _memoryCache.Programmes()
               .Where(a =>
               a.Channel != null &&
               (
                   epgids.Contains(a.Channel.ToLower()) ||
                   epgids.Contains(a.DisplayName.ToLower())
               )
               && a.Icon is not null && a.Icon.Count > 0 && a.Icon.Any(a => a.Src is not null)
               ).ToList();

        if (!programmes.Any()) { return; }

        foreach (var programme in programmes)
        {
            if (cancellationToken.IsCancellationRequested) { return; }

            if (programme is null || !programme.Icon.Any() || string.IsNullOrEmpty(programme.Icon[0].Src))
            {
                continue;
            }

            string source = HttpUtility.UrlDecode(programme.Icon[0].Src);
            var ext = Path.GetExtension(source);
            string name = string.Join("_", programme.Title.Text.Split(Path.GetInvalidFileNameChars())) + $".{ext}";
            string fileName = $"{FileDefinitions.ProgrammeIcon.DirectoryLocation}{name}";

            if (source.ToLower().StartsWith("https://json.schedulesdirect.org/20141201/image/"))
            {
                var sd = new SchedulesDirect();
                source = source.ToLower().Replace("https://json.schedulesdirect.org/20141201/image/", "");
                var result = await sd.GetImageUrl(source, fileName, cancellationToken).ConfigureAwait(false);
            }

            var iconDto =  IconHelper.AddIcon(source, programme.Title.Text, programme.EPGFileId, _mapper, _memoryCache, FileDefinitions.ProgrammeIcon, cancellationToken);
            if (iconDto is null)
            {
                continue;
            }
        }
    }
}
