namespace StreamMaster.Application.VideoStreams.Queries;

public record GetVideoStreamsForChannelGroups(List<int> ChannelGroupIds) : IRequest<List<VideoStreamDto>>;

internal class GetVideoStreamsForChannelGroupsHandler(ILogger<GetVideoStreamsForChannelGroups> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetVideoStreamsForChannelGroups, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(GetVideoStreamsForChannelGroups request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> ret = await Repository.VideoStream.GetVideoStreamsForChannelGroups(request.ChannelGroupIds, cancellationToken);
        return ret;
    }
}