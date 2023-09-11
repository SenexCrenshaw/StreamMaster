using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record SetVideoStreamsLogoFromEPGFromParametersRequest(VideoStreamParameters Parameters) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class SetVideoStreamsLogoFromEPGFromParametersRequestHandler : BaseMediatorRequestHandler, IRequestHandler<SetVideoStreamsLogoFromEPGFromParametersRequest, List<VideoStreamDto>>
{

    public SetVideoStreamsLogoFromEPGFromParametersRequestHandler(ILogger<SetVideoStreamsLogoFromEPGFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }


    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamsLogoFromEPGFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.SetVideoStreamsLogoFromEPGFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }
        return results;
    }
}