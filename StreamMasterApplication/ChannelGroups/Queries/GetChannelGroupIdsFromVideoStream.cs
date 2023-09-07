namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelIdFromVideoStream(VideoStreamDto VideoStreamDto) : IRequest<int?>;

internal class GetChannelIdFromVideoStreamHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelIdFromVideoStream, int?>
{

    public GetChannelIdFromVideoStreamHandler(ILogger<GetChannelIdFromVideoStream> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { }


    public async Task<int?> Handle(GetChannelIdFromVideoStream request, CancellationToken cancellationToken)
    {
        int? id = await Repository.ChannelGroup.GetChannelGroupIdFromVideoStream(request.VideoStreamDto.User_Tvg_group, cancellationToken).ConfigureAwait(false);
        return id;
    }
}