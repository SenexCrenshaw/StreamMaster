using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreams(VideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetVideoStreamsHandler(ILogger<GetVideoStreamsHandler> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetVideoStreams, PagedResponse<VideoStreamDto>>
{
    public async Task<PagedResponse<VideoStreamDto>> Handle(GetVideoStreams request, CancellationToken cancellationToken)
    {

        if (request.Parameters.PageSize == 0)
        {
            return request.Parameters.CreateEmptyPagedResponse<VideoStreamDto>();
        }

        PagedResponse<VideoStreamDto> res = await Repository.VideoStream.GetPagedVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);

        return res;
    }
}