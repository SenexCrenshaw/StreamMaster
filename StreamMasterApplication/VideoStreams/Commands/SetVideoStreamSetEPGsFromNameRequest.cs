using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public record SetVideoStreamSetEPGsFromNameRequest(List<string> VideoStreamIds) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class SetVideoStreamSetEPGsFromNameRequestHandler : BaseMemoryRequestHandler, IRequestHandler<SetVideoStreamSetEPGsFromNameRequest, List<VideoStreamDto>>
{

    public SetVideoStreamSetEPGsFromNameRequestHandler(ILogger<SetVideoStreamSetEPGsFromNameRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
    : base(logger, repository, mapper, publisher, sender, hubContext, memoryCache) { }


    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamSetEPGsFromNameRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.SetVideoStreamSetEPGsFromName(request.VideoStreamIds, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }
        return results;
    }
}
