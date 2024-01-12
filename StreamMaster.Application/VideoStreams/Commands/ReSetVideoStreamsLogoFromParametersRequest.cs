using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.VideoStreams.Commands;

public record ReSetVideoStreamsLogoFromParametersRequest(VideoStreamParameters Parameters) : IRequest { }

[LogExecutionTimeAspect]
public class ReSetVideoStreamsLogoFromParametersRequestHandler(ILogger<ReSetVideoStreamsLogoFromParametersRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<ReSetVideoStreamsLogoFromParametersRequest>
{
    public async Task Handle(ReSetVideoStreamsLogoFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.ReSetVideoStreamsLogoFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }
    }
}
