using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStream(string Id) : IRequest<VideoStreamDto?>;

internal class GetVideoStreamHandler : BaseRequestHandler, IRequestHandler<GetVideoStream, VideoStreamDto?>
{

    public GetVideoStreamHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService)
        : base(logger, repository, mapper, settingsService) { }


    public async Task<VideoStreamDto?> Handle(GetVideoStream request, CancellationToken cancellationToken)
    {
        return await Repository.VideoStream.GetVideoStreamById(request.Id).ConfigureAwait(false);

    }
}
