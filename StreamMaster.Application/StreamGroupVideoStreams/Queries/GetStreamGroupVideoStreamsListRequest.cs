namespace StreamMaster.Application.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamsListRequest(int StreamGroupId) : IRequest<List<VideoStreamDto>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupVideoStreamsListRequestHandler(ILogger<GetStreamGroupVideoStreamsListRequest> logger, IRepositoryWrapper Repository) : IRequestHandler<GetStreamGroupVideoStreamsListRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(GetStreamGroupVideoStreamsListRequest request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.StreamGroupId).ConfigureAwait(false);
    }
}