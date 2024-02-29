namespace StreamMaster.Application.VideoStreams.Queries;

public record GetVideoStream(string Id) : IRequest<VideoStreamDto?>;

internal class GetVideoStreamHandler(ILogger<GetVideoStreamHandler> logger, IRepositoryWrapper Repository) : IRequestHandler<GetVideoStream, VideoStreamDto?>
{
    public async Task<VideoStreamDto?> Handle(GetVideoStream request, CancellationToken cancellationToken)
    {
        return await Repository.VideoStream.GetVideoStreamById(request.Id).ConfigureAwait(false);

    }
}
