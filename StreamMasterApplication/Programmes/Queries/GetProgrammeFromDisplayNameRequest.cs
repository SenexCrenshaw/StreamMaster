using StreamMasterApplication.EPG.Queries;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeFromDisplayNameRequest(string value) : IRequest<ProgrammeNameDto?>;

internal class GetProgrammeFromDisplayNameHandler : BaseMediatorRequestHandler, IRequestHandler<GetProgrammeFromDisplayNameRequest, ProgrammeNameDto?>
{
    public GetProgrammeFromDisplayNameHandler(ILogger<GetProgrammeFromDisplayNameRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<ProgrammeNameDto?> Handle(GetProgrammeFromDisplayNameRequest request, CancellationToken cancellationToken)
    {
        ProgrammeNameDto? test = await Sender.Send(new GetEPGChannelByDisplayName(request.value), cancellationToken).ConfigureAwait(false);

        return test;
    }
}
