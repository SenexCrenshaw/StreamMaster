using StreamMaster.Domain.Models;

namespace StreamMaster.Application.VideoStreams.Queries;
public record GetVideoStreamInfoFromUrlRequest(string streamUrl) : IRequest<VideoInfo> { }


[LogExecutionTimeAspect]
public class GetVideoStreamInfoFromUrlHandler(ILogger<GetVideoStreamInfoFromUrlRequest> logger, IStreamManager streamManager) : IRequestHandler<GetVideoStreamInfoFromUrlRequest, VideoInfo>
{
    public async Task<VideoInfo> Handle(GetVideoStreamInfoFromUrlRequest request, CancellationToken cancellationToken)
    {
        return await streamManager.GetVideoInfo(request.streamUrl);
    }
}