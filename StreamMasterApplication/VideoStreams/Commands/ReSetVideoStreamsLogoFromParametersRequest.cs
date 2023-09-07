using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record ReSetVideoStreamsLogoFromParametersRequest(VideoStreamParameters Parameters) : IRequest { }

public class ReSetVideoStreamsLogoFromParametersRequestHandler(ILogger<ReSetVideoStreamsLogoFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<ReSetVideoStreamsLogoFromParametersRequest>
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
