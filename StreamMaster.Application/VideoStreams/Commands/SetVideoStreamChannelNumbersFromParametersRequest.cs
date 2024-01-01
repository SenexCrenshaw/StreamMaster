using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.VideoStreams.Commands;

public record SetVideoStreamChannelNumbersFromParametersRequest(VideoStreamParameters Parameters, bool OverWriteExisting, int StartNumber) : IRequest { }

[LogExecutionTimeAspect]
public class SetVideoStreamChannelNumbersFromParametersRequestHandler(ILogger<SetVideoStreamChannelNumbersFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SetVideoStreamChannelNumbersFromParametersRequest>
{
    public async Task Handle(SetVideoStreamChannelNumbersFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> streams = await Repository.VideoStream.SetVideoStreamChannelNumbersFromParameters(request.Parameters, request.OverWriteExisting, request.StartNumber, cancellationToken).ConfigureAwait(false);
        if (streams.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(streams), cancellationToken).ConfigureAwait(false);
        }
    }
}