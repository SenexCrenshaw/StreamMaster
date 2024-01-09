using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

public record VideoStreamChangeEPGNumberRequest(int OldEPGNumber, int NewEPGNumber) : IRequest { }

[LogExecutionTimeAspect]
public class VideoStreamChangeEPGNumberRequestHandler(ILogger<VideoStreamChangeEPGNumberRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<VideoStreamChangeEPGNumberRequest>
{
    public async Task Handle(VideoStreamChangeEPGNumberRequest request, CancellationToken cancellationToken)
    {
        if (request.OldEPGNumber == request.NewEPGNumber)
        {
            return;
        }

        List<VideoStreamDto> streams = await Repository.VideoStream.VideoStreamChangeEPGNumber(request.OldEPGNumber, request.NewEPGNumber, cancellationToken).ConfigureAwait(false);
        if (streams.Count != 0)
        {
            await HubContext.Clients.All.SchedulesDirectsRefresh();
            await Publisher.Publish(new UpdateVideoStreamsEvent(streams), cancellationToken).ConfigureAwait(false);
        }
    }
}