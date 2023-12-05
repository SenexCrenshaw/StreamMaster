using StreamMasterApplication.Programmes.Queries;

namespace StreamMasterApplication.EPG.Queries;

public record GetEPGChannelLogoByTvgId2(string User_Tvg_ID) : IRequest<string?>;

internal class GetEPGChannelLogoByTvgIdHandler(ILogger<GetEPGChannelLogoByTvgId2> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetEPGChannelLogoByTvgId2, string?>
{
    public async Task<string?> Handle(GetEPGChannelLogoByTvgId2 request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);
        List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == request.User_Tvg_ID || a.Channel == request.User_Tvg_ID || a.ChannelName == request.User_Tvg_ID);
        if (pn == null)
        {
            return null;
        }

        ChannelLogoDto? channelLogo = channelLogos.Find(a => a.EPGId == pn.Channel);
        if (channelLogo != null)
        {
            return channelLogo.Source;
        }
        return null;
    }
}
