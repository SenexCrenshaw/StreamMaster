using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamsRequest(StreamGroupVideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupVideoStreamsRequestHandler(ILogger<GetStreamGroupVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper) : BaseRequestHandler(logger, repository, mapper), IRequestHandler<GetStreamGroupVideoStreamsRequest, PagedResponse<VideoStreamDto>>
{
    public async Task<PagedResponse<VideoStreamDto>> Handle(GetStreamGroupVideoStreamsRequest request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
    }
}