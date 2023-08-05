using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

using System.Web;

namespace StreamMasterApplication.Icons.Commands;

public class BuildProgIconsCacheFromEPGsRequest : IRequest<bool>
{
}

public class BuildProgIconsCacheFromEPGsRequestHandler : BaseMemoryRequestHandler, IRequestHandler<BuildProgIconsCacheFromEPGsRequest, bool>
{

    public BuildProgIconsCacheFromEPGsRequestHandler(ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<bool> Handle(BuildProgIconsCacheFromEPGsRequest command, CancellationToken cancellationToken)
    {
        Setting setting = FileUtil.GetSetting();

        IEnumerable<EPGFile> epgFiles = await Repository.EPGFile.GetAllEPGFilesAsync();
        foreach (EPGFile? epg in epgFiles)
        {
            Tv? tv = await epg.GetTV();
            if (tv == null)
            {
                continue;
            }

            IEnumerable<TvChannel> epgChannels = tv.Channel.Where(a => a != null && a.Icon != null && a.Icon.Src != null && a.Icon.Src != "");

            if (!epgChannels.Any()) { continue; }

            WorkOnEPGChannelIcons(epg.Id, epgChannels, cancellationToken);
        }

        await WorkOnProgrammeIcons(cancellationToken).ConfigureAwait(false);

        return true;
    }

    private void WorkOnEPGChannelIcons(int epgFileId, IEnumerable<TvChannel> channels, CancellationToken cancellationToken)
    {
        foreach (TvChannel? channel in channels)
        {
            if (channel is null || channel.Icon == null || string.IsNullOrEmpty(channel.Icon.Src) || channel.Displayname == null || channel.Displayname[0] == null || channel.Displayname[0] == "")
            {
                continue;
            }

            if (cancellationToken.IsCancellationRequested) { return; }
            string icon = channel.Icon.Src;
            string source = HttpUtility.UrlDecode(icon);

            if (source.ToLower().StartsWith("https://json.schedulesdirect.org/20141201/image/"))
            {
            }
            else
            {
                string name = channel.Displayname != null ? channel.Displayname[0].ToString() : Path.GetFileNameWithoutExtension(source);
                IconFileDto? iconDto = IconHelper.AddIcon(source, name, epgFileId, Mapper, MemoryCache, FileDefinitions.ChannelIcon, cancellationToken);
                if (iconDto is null)
                {
                    continue;
                }

                List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
                if (!channelLogos.Any(a => a.LogoUrl == source))
                {
                    ChannelLogoDto cl = new() { LogoUrl = source, EPGId = channel.Id, EPGFileId = epgFileId };

                    MemoryCache.Add(cl);
                }
            }
        }
    }

    private async Task WorkOnProgrammeIcons(CancellationToken cancellationToken)
    {
        List<StreamGroupDto> sgs = await Repository.StreamGroup.GetStreamGroupDtos("", cancellationToken).ConfigureAwait(false);
        IEnumerable<string> epgids = sgs.SelectMany(x => x.ChildVideoStreams.Select(a => a.User_Tvg_ID)).Distinct();

        List<Programme> programmes = MemoryCache.Programmes()
               .Where(a =>
               a.Channel != null &&
               (
                   epgids.Contains(a.Channel.ToLower()) ||
                   epgids.Contains(a.DisplayName.ToLower())
               )
               && a.Icon is not null && a.Icon.Count > 0 && a.Icon.Any(a => a.Src is not null)
               ).ToList();

        if (!programmes.Any()) { return; }

        foreach (Programme? programme in programmes)
        {
            if (cancellationToken.IsCancellationRequested) { return; }

            if (programme is null || !programme.Icon.Any() || string.IsNullOrEmpty(programme.Icon[0].Src))
            {
                continue;
            }

            string source = HttpUtility.UrlDecode(programme.Icon[0].Src);
            string? ext = Path.GetExtension(source);
            string name = string.Join("_", programme.Title.Text.Split(Path.GetInvalidFileNameChars())) + $".{ext}";
            string fileName = $"{FileDefinitions.ProgrammeIcon.DirectoryLocation}{name}";

            if (source.ToLower().StartsWith("https://json.schedulesdirect.org/20141201/image/"))
            {
                SchedulesDirect sd = new();
                source = source.ToLower().Replace("https://json.schedulesdirect.org/20141201/image/", "");
                bool result = await sd.GetImageUrl(source, fileName, cancellationToken).ConfigureAwait(false);
            }

            IconFileDto? iconDto = IconHelper.AddIcon(source, programme.Title.Text, programme.EPGFileId, Mapper, MemoryCache, FileDefinitions.ProgrammeIcon, cancellationToken);
            if (iconDto is null)
            {
                continue;
            }
        }
    }
}
