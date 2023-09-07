namespace StreamMasterApplication.VideoStreamLinks.Commands;

[RequireAll]
public record RemoveVideoStreamFromVideoStreamRequest(string ParentVideoStreamId, string ChildVideoStreamId) : IRequest { }

public class RemoveVideoStreamFromVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<RemoveVideoStreamFromVideoStreamRequest>
{

    public RemoveVideoStreamFromVideoStreamRequestHandler(ILogger<RemoveVideoStreamFromVideoStreamRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
  : base(logger, repository, mapper, publisher, sender, hubContext) { }

    public async Task Handle(RemoveVideoStreamFromVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Repository.VideoStreamLink.RemoveVideoStreamFromVideoStream(request.ParentVideoStreamId, request.ChildVideoStreamId, cancellationToken);
    }
}