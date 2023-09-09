using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroups(StreamGroupParameters Parameters) : IRequest<PagedResponse<StreamGroupDto>>;

internal class GetStreamGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroups, PagedResponse<StreamGroupDto>>
{

    public GetStreamGroupsHandler(ILogger<GetStreamGroups> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { }
    public async Task<PagedResponse<StreamGroupDto>> Handle(GetStreamGroups request, CancellationToken cancellationToken = default)
    {

        if (request.Parameters.PageSize == 0)
        {
            return Repository.StreamGroup.CreateEmptyPagedResponse(request.Parameters);
        }


        return await Repository.StreamGroup.GetStreamGroupDtosPagedAsync(request.Parameters).ConfigureAwait(false);

    }
}