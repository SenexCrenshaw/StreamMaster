namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelIdFromVideoStream(VideoStreamDto VideoStreamDto) : IRequest<int?>;

internal class GetChannelIdFromVideoStreamHandler(ILogger<GetChannelIdFromVideoStream> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetChannelIdFromVideoStream, int?>
{
    public async Task<int?> Handle(GetChannelIdFromVideoStream request, CancellationToken cancellationToken)
    {
        int? id = await Repository.ChannelGroup.GetChannelGroupIdFromVideoStream(request.VideoStreamDto.User_Tvg_group).ConfigureAwait(false);
        return id;
    }
}