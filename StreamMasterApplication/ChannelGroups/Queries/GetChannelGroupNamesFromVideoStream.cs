namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupNameFromVideoStream(VideoStreamDto VideoStreamDto) : IRequest<string?>;

internal class GetChannelGroupNameFromVideoStreamHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupNameFromVideoStream, string?>
{

    public GetChannelGroupNameFromVideoStreamHandler(ILogger<GetChannelGroupNameFromVideoStream> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
 : base(logger, repository, mapper, publisher, sender, hubContext) { }
    public async Task<string?> Handle(GetChannelGroupNameFromVideoStream request, CancellationToken cancellationToken)
    {

        string? name = await Repository.ChannelGroup.GetChannelGroupNameFromVideoStream(request.VideoStreamDto.User_Tvg_group, cancellationToken).ConfigureAwait(false);
        return name;
    }
}