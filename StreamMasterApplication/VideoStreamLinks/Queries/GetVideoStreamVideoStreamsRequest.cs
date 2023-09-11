using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreamLinks.Queries;

public record GetVideoStreamVideoStreamsRequest(VideoStreamLinkParameters Parameters) : IRequest<PagedResponse<ChildVideoStreamDto>>;

internal class GetVideoStreamVideoStreamsRequestHandler(ILogger<GetVideoStreamVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetVideoStreamVideoStreamsRequest, PagedResponse<ChildVideoStreamDto>>
{
    public async Task<PagedResponse<ChildVideoStreamDto>> Handle(GetVideoStreamVideoStreamsRequest request, CancellationToken cancellationToken)
    {

        return await Repository.VideoStreamLink.GetVideoStreamVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);

    }
}