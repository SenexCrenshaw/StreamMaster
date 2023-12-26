using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

public record SetVideoStreamsLogoFromEPGRequest(List<string> Ids, string? OrderBy) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class SetVideoStreamsLogoFromEPGRequestHandler(ILogger<SetVideoStreamsLogoFromEPGRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SetVideoStreamsLogoFromEPGRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamsLogoFromEPGRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.SetVideoStreamsLogoFromEPGFromIds(request.Ids, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
