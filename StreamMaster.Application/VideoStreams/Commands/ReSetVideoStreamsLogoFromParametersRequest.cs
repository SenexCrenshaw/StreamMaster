using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

public record ReSetVideoStreamsLogoFromParametersRequest(VideoStreamParameters Parameters) : IRequest { }

[LogExecutionTimeAspect]
public class ReSetVideoStreamsLogoFromParametersRequestHandler(ILogger<ReSetVideoStreamsLogoFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<ReSetVideoStreamsLogoFromParametersRequest>
{
    public async Task Handle(ReSetVideoStreamsLogoFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.ReSetVideoStreamsLogoFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }
    }
}
