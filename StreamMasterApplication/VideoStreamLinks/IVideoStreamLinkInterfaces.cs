using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.VideoStreamLinks.Commands;
using StreamMasterApplication.VideoStreamLinks.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreamLinks;

public interface IVideoStreamLinkController
{
    Task<ActionResult<List<string>>> GetVideoStreamVideoStreamIds(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken);

    Task<ActionResult<List<ChildVideoStreamDto>>> GetVideoStreamVideoStreams(GetVideoStreamVideoStreamsRequest request, CancellationToken cancellationToken);

    Task<ActionResult> AddVideoStreamToVideoStream(AddVideoStreamToVideoStreamRequest request, CancellationToken cancellationToken);

    Task<ActionResult> RemoveVideoStreamFromVideoStream(RemoveVideoStreamFromVideoStreamRequest request, CancellationToken cancellationToken);
}

public interface IVideoStreamLinkHub
{
    Task RemoveVideoStreamFromVideoStream(RemoveVideoStreamFromVideoStreamRequest request, CancellationToken cancellationToken);

    Task AddVideoStreamToVideoStream(AddVideoStreamToVideoStreamRequest request, CancellationToken cancellationToken);

    Task<List<string>> GetVideoStreamVideoStreamIds(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken);

    Task<List<ChildVideoStreamDto>> GetVideoStreamVideoStreams(GetVideoStreamVideoStreamsRequest request, CancellationToken cancellationToken);
}