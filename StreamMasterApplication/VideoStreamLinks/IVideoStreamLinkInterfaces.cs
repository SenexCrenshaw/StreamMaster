using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.VideoStreamLinks.Commands;
using StreamMasterApplication.VideoStreamLinks.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreamLinks;

public interface IVideoStreamLinkController
{
    Task<ActionResult<List<string>>> GetVideoStreamVideoStreamIds(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken);

    Task<ActionResult<PagedResponse<VideoStreamDto>>> GetPagedVideoStreamVideoStreams([FromQuery] VideoStreamLinkParameters Parameters, CancellationToken cancellationToken);

    Task<ActionResult> AddVideoStreamToVideoStream(AddVideoStreamToVideoStreamRequest request);

    Task<ActionResult> RemoveVideoStreamFromVideoStream(RemoveVideoStreamFromVideoStreamRequest request);
}

public interface IVideoStreamLinkHub
{
    Task RemoveVideoStreamFromVideoStream(RemoveVideoStreamFromVideoStreamRequest request);

    Task AddVideoStreamToVideoStream(AddVideoStreamToVideoStreamRequest request);

    Task<List<string>> GetVideoStreamVideoStreamIds(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken);

    Task<PagedResponse<VideoStreamDto>> GetPagedVideoStreamVideoStreams(VideoStreamLinkParameters Parameters, CancellationToken cancellationToken);
}