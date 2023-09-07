namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroupsByVideoStreamIdsQuery(List<string> VideoStreamIds) : IRequest<IEnumerable<StreamGroupDto>>;

internal class GetStreamGroupsByVideoStreamIdsQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroupsByVideoStreamIdsQuery, IEnumerable<StreamGroupDto>>
{


    public GetStreamGroupsByVideoStreamIdsQueryHandler(ILogger<GetStreamGroupsByVideoStreamIdsQuery> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
 : base(logger, repository, mapper, publisher, sender, hubContext) { }


    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroupsByVideoStreamIdsQuery request, CancellationToken cancellationToken = default)
    {

        List<StreamGroupDto> sgs = await Repository.StreamGroup.GetStreamGroupDtos(cancellationToken);

        List<StreamGroupDto> matchingStreamGroups = sgs
            .Where(sg => sg.ChildVideoStreams.Any(sgvs => request.VideoStreamIds.Contains(sgvs.Id)))
            .ToList();

        return matchingStreamGroups;

    }
}
