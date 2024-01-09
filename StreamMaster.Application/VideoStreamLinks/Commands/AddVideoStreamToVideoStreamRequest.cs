namespace StreamMaster.Application.VideoStreamLinks.Commands;

[RequireAll]
public record AddVideoStreamToVideoStreamRequest(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank) : IRequest { }

public class AddVideoStreamToVideoStreamRequestHandler(ILogger<AddVideoStreamToVideoStreamRequest> logger, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<AddVideoStreamToVideoStreamRequest>
{
    public async Task Handle(AddVideoStreamToVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Repository.VideoStreamLink.AddVideoStreamTodVideoStream(request.ParentVideoStreamId, request.ChildVideoStreamId, request.Rank, cancellationToken).ConfigureAwait(false);
        await HubContext.Clients.All.VideoStreamLinksRefresh([request.ChildVideoStreamId]);
    }
}