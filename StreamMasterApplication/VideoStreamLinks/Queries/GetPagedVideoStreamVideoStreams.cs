using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreamLinks.Queries;

public record GetPagedVideoStreamVideoStreams(VideoStreamLinkParameters Parameters) : IRequest<PagedResponse<ChildVideoStreamDto>>;

internal class GetPagedVideoStreamVideoStreamsHandler(ILogger<GetPagedVideoStreamVideoStreams> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetPagedVideoStreamVideoStreams, PagedResponse<ChildVideoStreamDto>>
{
    public async Task<PagedResponse<ChildVideoStreamDto>> Handle(GetPagedVideoStreamVideoStreams request, CancellationToken cancellationToken)
    {

        return await Repository.VideoStreamLink.GetVideoStreamVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);

    }
}