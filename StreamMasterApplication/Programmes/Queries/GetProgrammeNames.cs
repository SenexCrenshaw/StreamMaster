namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeNames : IRequest<List<string>>;

internal class GetProgrammeNamesHandler : BaseMemoryRequestHandler, IRequestHandler<GetProgrammeNames, List<string>>
{

    public GetProgrammeNamesHandler(ILogger<GetProgrammeNames> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
    : base(logger, repository, mapper, publisher, sender, hubContext, memoryCache) { }

    public Task<List<string>> Handle(GetProgrammeNames request, CancellationToken cancellationToken)
    {
        List<string> programmes = MemoryCache.Programmes()
            .Where(a => !string.IsNullOrEmpty(a.Channel))
            .Select(a => a.DisplayName).Distinct().ToList();

        return Task.FromResult(programmes);
    }
}
