namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFilePreviewById(int Id) : IRequest<List<EPGFilePreviewDto>>;

internal class GetEPGFilePreviewByIdHandler : BaseMediatorRequestHandler, IRequestHandler<GetEPGFilePreviewById, List<EPGFilePreviewDto>>
{

    public GetEPGFilePreviewByIdHandler(ILogger<GetEPGFilePreviewById> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<List<EPGFilePreviewDto>> Handle(GetEPGFilePreviewById request, CancellationToken cancellationToken = default)
    {
        var res = await Repository.EPGFile.GetEPGFilePreviewById(request.Id, cancellationToken).ConfigureAwait(false);

        return res;
    }
}
