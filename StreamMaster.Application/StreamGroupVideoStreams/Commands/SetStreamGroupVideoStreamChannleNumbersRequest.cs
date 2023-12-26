using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.VideoStreams.Commands;

namespace StreamMaster.Application.StreamGroupVideoStreams.Commands;

public record SetStreamGroupVideoStreamChannelNumbersRequest(int StreamGroupId, int StartingNumber, string OrderBy) : IRequest { }

[LogExecutionTimeAspect]
public class SetStreamGroupVideoStreamChannelNumbersHandler(ILogger<SetStreamGroupVideoStreamChannelNumbersRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SetStreamGroupVideoStreamChannelNumbersRequest>
{
    public async Task Handle(SetStreamGroupVideoStreamChannelNumbersRequest request, CancellationToken cancellationToken)
    {

        List<VideoStreamIsReadOnly> vidIds = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreamIds(request.StreamGroupId, cancellationToken).ConfigureAwait(false);

        if (vidIds.Any())
        {
            await Sender.Send(new SetVideoStreamChannelNumbersRequest(vidIds.ConvertAll(x => x.VideoStreamId), true, request.StartingNumber, request.OrderBy), cancellationToken).ConfigureAwait(false);
        }
    }
}
