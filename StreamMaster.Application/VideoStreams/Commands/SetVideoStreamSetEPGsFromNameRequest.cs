using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

public record SetVideoStreamSetEPGsFromNameRequest(List<string> VideoStreamIds) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class SetVideoStreamSetEPGsFromNameRequestHandler(ILogger<SetVideoStreamSetEPGsFromNameRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<SetVideoStreamSetEPGsFromNameRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamSetEPGsFromNameRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.SetVideoStreamSetEPGsFromName(request.VideoStreamIds, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }
        return results;
    }
}
