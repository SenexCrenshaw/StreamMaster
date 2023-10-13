using StreamMasterApplication.Programmes.Queries;

namespace StreamMasterApplication.EPG.Queries;

public record GetEPGChannelLogoByTvgId(string User_Tvg_ID) : IRequest<string?>;

internal class GetEPGChannelLogoByTvgIdHandler(ILogger<GetEPGChannelLogoByTvgId> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetEPGChannelLogoByTvgId, string?>
{
    public async Task<string?> Handle(GetEPGChannelLogoByTvgId request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);
        List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == request.User_Tvg_ID || a.Channel == request.User_Tvg_ID || a.ChannelName == request.User_Tvg_ID);
        if (pn == null)
        {
            return null;
        }

        ChannelLogoDto? channelLogo = channelLogos.FirstOrDefault(a => a.EPGId == pn.Channel);
        if (channelLogo != null)
        {
            return channelLogo.LogoUrl;
        }
        return null;
    }
}
