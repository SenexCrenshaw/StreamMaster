namespace StreamMaster.Application.VideoStreams.Queries;
public record GetVideoStreamInfoFromIdRequest(string channelVideoStreamId) : IRequest<VideoInfo> { }


[LogExecutionTimeAspect]
public class GetVideoStreamInfoFromIdRequestHandler(ILogger<GetVideoStreamInfoFromIdRequest> logger, IChannelManager channelManager) : IRequestHandler<GetVideoStreamInfoFromIdRequest, VideoInfo>
{
    public Task<VideoInfo> Handle(GetVideoStreamInfoFromIdRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(channelManager.GetVideoInfo(request.channelVideoStreamId));
    }
}