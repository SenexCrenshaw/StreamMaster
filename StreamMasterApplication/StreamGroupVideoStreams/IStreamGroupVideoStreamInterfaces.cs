using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupVideoStreams.Commands;
using StreamMasterApplication.StreamGroupVideoStreams.Queries;
using StreamMasterApplication.VideoStreamLinks.Commands;
using StreamMasterApplication.VideoStreamLinks.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroupVideoStreams;

public interface IStreamGroupVideoStreamController
{
    Task<ActionResult<List<VideoStreamDto>>> GetStreamGroupVideoStreams(GetStreamGroupVideoStreamsRequest request, CancellationToken cancellationToken = default);
    Task<ActionResult<List<VideoStreamIsReadOnly>>> GetStreamGroupVideoStreamIds(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default);

    Task<ActionResult> AddVideoStreamToStreamGroup(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);
    Task<ActionResult> RemoveVideoStreamToStreamGroup(RemoveVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);


}

public interface IStreamGroupVideoStreamHub
{

    Task RemoveVideoStreamToStreamGroup(RemoveVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);

    Task AddVideoStreamToStreamGroup(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);

    Task<List<VideoStreamDto>> GetStreamGroupVideoStreams(GetStreamGroupVideoStreamsRequest request, CancellationToken cancellationToken = default);
    Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default);
}