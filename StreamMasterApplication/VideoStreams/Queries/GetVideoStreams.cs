using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetPagedVideoStreams(VideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetPagedVideoStreamsHandler(ILogger<GetPagedVideoStreamsHandler> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetPagedVideoStreams, PagedResponse<VideoStreamDto>>
{
    public async Task<PagedResponse<VideoStreamDto>> Handle(GetPagedVideoStreams request, CancellationToken cancellationToken)
    {

        if (request.Parameters.PageSize == 0)
        {
            return Repository.VideoStream.CreateEmptyPagedResponse();
        }

        PagedResponse<VideoStreamDto> res = await Repository.VideoStream.GetPagedVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);

        return res;
    }
}