using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.VideoStreams.Commands;

public record SetVideoStreamsLogoFromEPGFromParametersRequest(VideoStreamParameters Parameters) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class SetVideoStreamsLogoFromEPGFromParametersRequestHandler(ILogger<SetVideoStreamsLogoFromEPGFromParametersRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<SetVideoStreamsLogoFromEPGFromParametersRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamsLogoFromEPGFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.SetVideoStreamsLogoFromEPGFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }
        return results;
    }
}