using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.VideoStreams.Commands;

public record SetVideoStreamTimeShiftFromParametersRequest(VideoStreamParameters Parameters, int TimeShift) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class SetVideoStreamTimeShiftFromParametersRequestHandler(ILogger<SetVideoStreamTimeShiftFromParametersRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<SetVideoStreamTimeShiftFromParametersRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamTimeShiftFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.SetVideoStreamTimeShiftFromParameters(request.Parameters, request.TimeShift, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }
        return results;
    }
}