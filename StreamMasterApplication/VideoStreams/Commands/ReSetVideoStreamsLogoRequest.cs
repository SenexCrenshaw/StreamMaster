using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public record ReSetVideoStreamsLogoRequest(List<string> Ids) : IRequest<List<VideoStreamDto>> { }

public class ReSetVideoStreamsLogoRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ReSetVideoStreamsLogoRequest, List<VideoStreamDto>>
{
    public ReSetVideoStreamsLogoRequestHandler(ILogger<ReSetVideoStreamsLogoRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
 : base(logger, repository, mapper, publisher, sender, hubContext) { }


    public async Task<List<VideoStreamDto>> Handle(ReSetVideoStreamsLogoRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.ReSetVideoStreamsLogoFromIds(request.Ids, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
