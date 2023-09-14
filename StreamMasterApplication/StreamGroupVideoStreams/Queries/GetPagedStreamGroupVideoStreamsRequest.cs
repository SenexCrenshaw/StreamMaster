using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroupVideoStreams.Queries;

public record GetPagedStreamGroupVideoStreamsRequest(StreamGroupVideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupVideoStreamsRequestHandler(ILogger<GetPagedStreamGroupVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetPagedStreamGroupVideoStreamsRequest, PagedResponse<VideoStreamDto>>
{
    public async Task<PagedResponse<VideoStreamDto>> Handle(GetPagedStreamGroupVideoStreamsRequest request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetPagedStreamGroupVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
    }
}