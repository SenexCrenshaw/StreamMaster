using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.VideoStreams.Commands;

public record AutoSetEPGFromParametersRequest(VideoStreamParameters Parameters, List<string> Ids) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class AutoSetEPGFromParametersRequestHandler(ILogger<AutoSetEPGFromParametersRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<AutoSetEPGFromParametersRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(AutoSetEPGFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.AutoSetEPGFromParameters(request.Parameters, request.Ids, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);

        }

        return results;
    }
}
