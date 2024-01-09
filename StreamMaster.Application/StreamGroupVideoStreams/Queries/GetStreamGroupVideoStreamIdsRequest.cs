namespace StreamMaster.Application.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamIdsRequest(int StreamGroupId) : IRequest<List<VideoStreamIsReadOnly>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupVideoStreamIdsRequestHandler(ILogger<GetStreamGroupVideoStreamIdsRequestHandler> logger, IRepositoryWrapper Repository) : IRequestHandler<GetStreamGroupVideoStreamIdsRequest, List<VideoStreamIsReadOnly>>
{
    public async Task<List<VideoStreamIsReadOnly>> Handle(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreamIds(request.StreamGroupId, cancellationToken).ConfigureAwait(false);
    }
}