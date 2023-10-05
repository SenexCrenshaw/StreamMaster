using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public record AutoSetEPGRequest(List<string> Ids) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class AutoSetEPGRequestHandler(ILogger<AutoSetEPGRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<AutoSetEPGRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(AutoSetEPGRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.AutoSetEPGFromIds(request.Ids, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);

        }

        return results;
    }
}
