using FluentValidation;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Repository.EPG;

using System.Web;

namespace StreamMasterApplication.Icons.Commands;

public class BuildProgIconsCacheFromEPGsRequest : IRequest<bool> { }

public class BuildProgIconsCacheFromEPGsRequestHandler(ILogger<BuildProgIconsCacheFromEPGsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMemoryRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<BuildProgIconsCacheFromEPGsRequest, bool>
{
    public async Task<bool> Handle(BuildProgIconsCacheFromEPGsRequest command, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync();

        int startId = MemoryCache.GetIcons(Mapper).Count;

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

            WorkOnEPGChannelIcons(epg.Id, startId, epgChannels, cancellationToken);
        }

        await WorkOnProgrammeIcons(startId, cancellationToken).ConfigureAwait(false);

        return true;
    }

    private void WorkOnEPGChannelIcons(int epgFileId, int startId, IEnumerable<TvChannel> channels, CancellationToken cancellationToken)
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
                IconFileDto? iconDto = IconHelper.AddIcon(source, name, epgFileId, startId++, MemoryCache, FileDefinitions.ChannelIcon, cancellationToken);
                if (iconDto is null)
                {
                    continue;
                }

                List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
                if (!channelLogos.Any(a => a.LogoUrl == source))
                {
                    ChannelLogoDto cl = new()
                    {
                        Id = iconDto.Id,
                        LogoUrl = source,
                        EPGId = channel.Id,
                        EPGFileId = epgFileId
                    };

                    MemoryCache.Add(cl);
                }
            }
        }
    }

    private async Task WorkOnProgrammeIcons(int startId, CancellationToken cancellationToken)
    {
        List<string> epgids = await Repository.StreamGroup
        .FindAll()
        .Include(a => a.ChildVideoStreams)
        .SelectMany(a => a.ChildVideoStreams)
        .Select(a => a.ChildVideoStream.User_Tvg_ID)
        .Distinct()
        .AsNoTracking() // Only add this if you're using Entity Framework Core and you don't need to track the entities.
        .ToListAsync(cancellationToken: cancellationToken)
        .ConfigureAwait(false);


        //List<StreamGroupDto> sgs = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreamIds();

        //IEnumerable<string> epgids = sgs.SelectMany(x => x.ChildVideoStreams.Select(a => a.User_Tvg_ID)).Distinct();

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
            bool result = true;

            if (source.ToLower().StartsWith("https://json.schedulesdirect.org/20141201/image/"))
            {
                continue;
                //SchedulesDirect sd = new();
                //source = source.ToLower().Replace("https://json.schedulesdirect.org/20141201/image/", "");
                //result = await sd.GetImageUrl(source, fileName, cancellationToken).ConfigureAwait(false);
            }

            if (result)
            {
                IconFileDto? iconDto = IconHelper.AddIcon(source, programme.Title.Text, programme.EPGFileId, startId, MemoryCache, FileDefinitions.ProgrammeIcon, cancellationToken);
                if (iconDto is null)
                {
                    continue;
                }
            }
        }
    }
}
