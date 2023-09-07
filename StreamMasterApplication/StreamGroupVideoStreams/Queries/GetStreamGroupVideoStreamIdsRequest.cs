namespace StreamMasterApplication.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamIdsRequest(int StreamGroupId) : IRequest<List<VideoStreamIsReadOnly>>;

internal class GetStreamGroupVideoStreamIdsRequestHandler(ILogger<GetStreamGroupVideoStreamIdsRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper) : BaseRequestHandler(logger, repository, mapper), IRequestHandler<GetStreamGroupVideoStreamIdsRequest, List<VideoStreamIsReadOnly>>
{
    public async Task<List<VideoStreamIsReadOnly>> Handle(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreamIds(request.StreamGroupId, cancellationToken).ConfigureAwait(false);
    }
}