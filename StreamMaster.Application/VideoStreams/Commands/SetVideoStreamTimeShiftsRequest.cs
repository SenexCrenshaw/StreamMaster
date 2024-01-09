using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

[RequireAll]
public record SetVideoStreamTimeShiftsRequest(List<string> Ids, string TimeShift) : IRequest { }

[LogExecutionTimeAspect]
public class SetVideoStreamTimeShiftsRequestHandler(ILogger<SetVideoStreamTimeShiftsRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<SetVideoStreamTimeShiftsRequest>
{
    public async Task Handle(SetVideoStreamTimeShiftsRequest request, CancellationToken cancellationToken)
    {

        List<VideoStreamDto> streams = await Repository.VideoStream.SetVideoStreamTimeShiftsFromIds(request.Ids, request.TimeShift, cancellationToken).ConfigureAwait(false);
        if (streams.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(streams), cancellationToken).ConfigureAwait(false);
        }
    }
}