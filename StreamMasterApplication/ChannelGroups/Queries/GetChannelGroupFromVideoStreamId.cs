namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupFromVideoStreamId(string VideoStreamId) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupFromVideoStreamIdHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupFromVideoStreamId, ChannelGroupDto?>
{

    public GetChannelGroupFromVideoStreamIdHandler(ILogger<GetChannelGroupFromVideoStreamId> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
 : base(logger, repository, mapper, publisher, sender, hubContext) { }
    public async Task<ChannelGroupDto?> Handle(GetChannelGroupFromVideoStreamId request, CancellationToken cancellationToken)
    {
        ChannelGroupDto? res = await Repository.ChannelGroup.GetChannelGroupFromVideoStreamId(request.VideoStreamId, cancellationToken).ConfigureAwait(false);
        return res;
    }
}