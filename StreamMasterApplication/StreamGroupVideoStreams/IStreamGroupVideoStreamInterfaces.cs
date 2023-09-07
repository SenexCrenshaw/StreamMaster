using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupVideoStreams.Commands;
using StreamMasterApplication.StreamGroupVideoStreams.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroupVideoStreams;

public interface IStreamGroupVideoStreamController
{
    Task<ActionResult> SetVideoStreamRanks(SetVideoStreamRanksRequest request, CancellationToken cancellationToken);
    Task<ActionResult<PagedResponse<VideoStreamDto>>> GetStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default);

    Task<ActionResult<List<VideoStreamIsReadOnly>>> GetStreamGroupVideoStreamIds(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default);

    Task<ActionResult> AddVideoStreamToStreamGroup(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);

    Task<ActionResult> RemoveVideoStreamFromStreamGroup(RemoveVideoStreamFromStreamGroupRequest request, CancellationToken cancellationToken);
}

public interface IStreamGroupVideoStreamHub
{
    Task SetVideoStreamRanks(SetVideoStreamRanksRequest request, CancellationToken cancellationToken);
    Task RemoveVideoStreamFromStreamGroup(RemoveVideoStreamFromStreamGroupRequest request, CancellationToken cancellationToken);

    Task AddVideoStreamToStreamGroup(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);

    Task<PagedResponse<VideoStreamDto>> GetStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default);

    Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default);
}