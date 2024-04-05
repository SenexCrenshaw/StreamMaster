namespace StreamMaster.Application.VideoStreamLinks.Queries;

public record GetPagedVideoStreamVideoStreams(VideoStreamLinkParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetPagedVideoStreamVideoStreamsHandler(ILogger<GetPagedVideoStreamVideoStreams> logger, IRepositoryWrapper Repository) : IRequestHandler<GetPagedVideoStreamVideoStreams, PagedResponse<VideoStreamDto>>
{
    public async Task<PagedResponse<VideoStreamDto>> Handle(GetPagedVideoStreamVideoStreams request, CancellationToken cancellationToken)
    {
        return string.IsNullOrEmpty(request.Parameters.JSONFiltersString) || !request.Parameters.JSONFiltersString.Contains("parentVideoStreamId")
            ? Repository.VideoStreamLink.CreateEmptyPagedResponse()
            : await Repository.VideoStreamLink.GetVideoStreamVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
    }
}