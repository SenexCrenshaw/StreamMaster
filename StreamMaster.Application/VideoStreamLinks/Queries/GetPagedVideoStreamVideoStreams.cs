using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.VideoStreamLinks.Queries;

public record GetPagedVideoStreamVideoStreams(VideoStreamLinkParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetPagedVideoStreamVideoStreamsHandler(ILogger<GetPagedVideoStreamVideoStreams> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetPagedVideoStreamVideoStreams, PagedResponse<VideoStreamDto>>
{
    public async Task<PagedResponse<VideoStreamDto>> Handle(GetPagedVideoStreamVideoStreams request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Parameters.JSONFiltersString) || !request.Parameters.JSONFiltersString.Contains("parentVideoStreamId"))
        {
            return repository.VideoStreamLink.CreateEmptyPagedResponse();
        }

        return await Repository.VideoStreamLink.GetVideoStreamVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
    }
}