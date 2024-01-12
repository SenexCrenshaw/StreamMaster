using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

public record AutoSetEPGRequest(List<string> Ids) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class AutoSetEPGRequestHandler(ILogger<AutoSetEPGRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<AutoSetEPGRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(AutoSetEPGRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.AutoSetEPGFromIds(request.Ids, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);

        }

        return results;
    }
}
