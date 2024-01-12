using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.StreamGroupVideoStreams.Queries;

public record GetPagedStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupVideoStreamsHandler(ILogger<GetPagedStreamGroupVideoStreams> logger, IRepositoryWrapper Repository) : IRequestHandler<GetPagedStreamGroupVideoStreams, PagedResponse<VideoStreamDto>>
{
    public async Task<PagedResponse<VideoStreamDto>> Handle(GetPagedStreamGroupVideoStreams request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetPagedStreamGroupVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
    }
}