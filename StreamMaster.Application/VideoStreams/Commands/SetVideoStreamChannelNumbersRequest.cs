using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

[RequireAll]
public record SetVideoStreamChannelNumbersRequest(List<string> Ids, bool OverWriteExisting, int StartNumber, string OrderBy) : IRequest { }

[LogExecutionTimeAspect]
public class SetVideoStreamChannelNumbersRequestHandler(ILogger<SetVideoStreamChannelNumbersRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<SetVideoStreamChannelNumbersRequest>
{
    public async Task Handle(SetVideoStreamChannelNumbersRequest request, CancellationToken cancellationToken)
    {
        string orderBy = string.IsNullOrEmpty(request.OrderBy) ? "user_tvg_name desc" : request.OrderBy;
        List<VideoStreamDto> streams = await Repository.VideoStream.SetVideoStreamChannelNumbersFromIds(request.Ids, request.OverWriteExisting, request.StartNumber, orderBy, cancellationToken).ConfigureAwait(false);
        if (streams.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(streams), cancellationToken).ConfigureAwait(false);
        }
    }
}