using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreams(VideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetVideoStreamsHandler(ILogger<GetVideoStreamsHandler> logger, IRepositoryWrapper repository, IMapper mapper) : BaseRequestHandler(logger, repository, mapper), IRequestHandler<GetVideoStreams, PagedResponse<VideoStreamDto>>
{
    public async Task<PagedResponse<VideoStreamDto>> Handle(GetVideoStreams request, CancellationToken cancellationToken)
    {
        int count = Repository.StreamGroup.Count();

        if (request.Parameters.PageSize == 0)
        {
            PagedResponse<VideoStreamDto> emptyResponse = new();
            emptyResponse.TotalItemCount = count;
            return emptyResponse;

        }

        PagedResponse<VideoStreamDto> res = await Repository.VideoStream.GetVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);

        return res;
    }
}