namespace StreamMasterApplication.VideoStreamLinks.Commands;

[RequireAll]
public record AddVideoStreamToVideoStreamRequest(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank) : IRequest { }

public class AddVideoStreamToVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<AddVideoStreamToVideoStreamRequest>
{

    public AddVideoStreamToVideoStreamRequestHandler(ILogger<AddVideoStreamToVideoStreamRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task Handle(AddVideoStreamToVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Repository.VideoStreamLink.AddVideoStreamTodVideoStream(request.ParentVideoStreamId, request.ChildVideoStreamId, request.Rank, cancellationToken).ConfigureAwait(false);

    }
}