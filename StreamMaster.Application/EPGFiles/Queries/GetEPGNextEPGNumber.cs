namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGNextEPGNumber() : IRequest<int>;

internal class GetEPGNextEPGNumberHandler(ILogger<GetEPGNextEPGNumber> logger, IRepositoryWrapper repository, ISchedulesDirectDataService schedulesDirectDataService, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetEPGNextEPGNumber, int>
{
    public async Task<int> Handle(GetEPGNextEPGNumber request, CancellationToken cancellationToken = default)
    {
        var nextAvailableNumber = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);

        return nextAvailableNumber;
    }
}
