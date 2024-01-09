namespace StreamMaster.Application.VideoStreamLinks.Commands;

[RequireAll]
public record RemoveVideoStreamFromVideoStreamRequest(string ParentVideoStreamId, string ChildVideoStreamId) : IRequest { }

public class RemoveVideoStreamFromVideoStreamRequestHandler(ILogger<RemoveVideoStreamFromVideoStreamRequest> logger, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<RemoveVideoStreamFromVideoStreamRequest>
{
    public async Task Handle(RemoveVideoStreamFromVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Repository.VideoStreamLink.RemoveVideoStreamFromVideoStream(request.ParentVideoStreamId, request.ChildVideoStreamId, cancellationToken);
        await HubContext.Clients.All.VideoStreamLinksRemove([request.ChildVideoStreamId]);
    }
}