using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

[RequireAll]
public record SetVideoStreamChannelNumbersRequest(List<string> Ids, bool OverWriteExisting, int StartNumber, string OrderBy) : IRequest { }

[LogExecutionTimeAspect]
public class SetVideoStreamChannelNumbersRequestHandler : BaseMediatorRequestHandler, IRequestHandler<SetVideoStreamChannelNumbersRequest>
{

    public SetVideoStreamChannelNumbersRequestHandler(ILogger<SetVideoStreamChannelNumbersRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    : base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }
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