namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeFromDisplayName(string Tvg_ID) : IRequest<ProgrammeNameDto?>;

internal class GetProgrammeFromDisplayNameHandler : BaseMemoryRequestHandler, IRequestHandler<GetProgrammeFromDisplayName, ProgrammeNameDto?>
{
    public GetProgrammeFromDisplayNameHandler(ILogger<GetProgrammeFromDisplayName> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache) { }

    public Task<ProgrammeNameDto?> Handle(GetProgrammeFromDisplayName request, CancellationToken cancellationToken)
    {
        ProgrammeNameDto? test = MemoryCache.GetEPGChannelByDisplayName(request.Tvg_ID);

        return Task.FromResult(test);
    }
}
