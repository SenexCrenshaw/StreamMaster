using Microsoft.AspNetCore.Http;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroup(int Id) : IRequest<StreamGroupDto?>;

internal class GetStreamGroupHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroup, StreamGroupDto?>
{

    public GetStreamGroupHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroup> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
  : base(logger, repository, mapper, publisher, sender, hubContext) { }

    public async Task<StreamGroupDto?> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {
        if (request.Id == 0)
        {
            return new StreamGroupDto { Id = 0, Name = "All" };
        }


        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDto(request.Id, cancellationToken).ConfigureAwait(false);
        return streamGroup;
    }
}
