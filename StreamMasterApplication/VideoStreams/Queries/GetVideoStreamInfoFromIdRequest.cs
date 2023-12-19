namespace StreamMasterApplication.VideoStreams.Queries;
public record GetVideoStreamInfoFromIdRequest(string channelVideoStreamId) : IRequest<VideoInfo> { }


[LogExecutionTimeAspect]
public class GetVideoStreamInfoFromIdRequestHandler(ILogger<GetVideoStreamInfoFromIdRequest> logger, IChannelManager channelManager) : IRequestHandler<GetVideoStreamInfoFromIdRequest, VideoInfo>
{
    public async Task<VideoInfo> Handle(GetVideoStreamInfoFromIdRequest request, CancellationToken cancellationToken)
    {
        return await channelManager.GetVideoInfo(request.channelVideoStreamId);
    }
}