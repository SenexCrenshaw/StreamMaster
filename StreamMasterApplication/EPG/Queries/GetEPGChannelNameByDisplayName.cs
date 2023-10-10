using StreamMasterApplication.Programmes.Queries;

namespace StreamMasterApplication.EPG.Queries;

public record GetEPGChannelNameByDisplayName(string displayName) : IRequest<string?>;

internal class GetEPGChannelNameByDisplayNameHandler(ILogger<GetEPGChannelNameByDisplayName> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetEPGChannelNameByDisplayName, string?>
{
    public async Task<string?> Handle(GetEPGChannelNameByDisplayName request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);
        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == request.displayName);
        if (pn == null)
        {
            pn = programmeNames.FirstOrDefault(a => a.ChannelName == request.displayName);
            if (pn == null)
            {
                return null;
            }
        }
        return pn.Channel;
    }
}
