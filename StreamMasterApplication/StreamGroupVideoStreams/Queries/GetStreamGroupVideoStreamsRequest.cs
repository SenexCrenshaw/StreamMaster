using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamsRequest(StreamGroupVideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetStreamGroupVideoStreamsRequestHandler : BaseRequestHandler, IRequestHandler<GetStreamGroupVideoStreamsRequest, PagedResponse<VideoStreamDto>>
{
    public GetStreamGroupVideoStreamsRequestHandler(ILogger<GetStreamGroupVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<PagedResponse<VideoStreamDto>> Handle(GetStreamGroupVideoStreamsRequest request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
    }
}