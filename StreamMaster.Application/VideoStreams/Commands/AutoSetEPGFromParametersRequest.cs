using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

public record AutoSetEPGFromParametersRequest(VideoStreamParameters Parameters, List<string> Ids) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class AutoSetEPGFromParametersRequestHandler(ILogger<AutoSetEPGFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<AutoSetEPGFromParametersRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(AutoSetEPGFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.AutoSetEPGFromParameters(request.Parameters, request.Ids, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);

        }

        return results;
    }
}
