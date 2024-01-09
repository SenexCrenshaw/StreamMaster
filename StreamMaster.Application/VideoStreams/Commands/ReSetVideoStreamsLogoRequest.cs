using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

public record ReSetVideoStreamsLogoRequest(List<string> Ids) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class ReSetVideoStreamsLogoRequestHandler(ILogger<ReSetVideoStreamsLogoRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<ReSetVideoStreamsLogoRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(ReSetVideoStreamsLogoRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.ReSetVideoStreamsLogoFromIds(request.Ids, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
